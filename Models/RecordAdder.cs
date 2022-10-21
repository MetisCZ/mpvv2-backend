using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using mpvv2.DbModels;

namespace mpvv2.Models
{
    public class RecordAdder
    {

        private static readonly HttpClient client = new HttpClient();

        public RecordAdder()
        {
        }
        
        public void GenerateRecord(int type)
        {
            type = 1;
            JsonDocument json = null;
            switch (type)
            {
                case 1:
                    json = GetJsonFromSiteOdis();
                    break;
                case 2:
                    json = GetJsonFromSitePid();
                    break;
            }
            var list = CreateList(json, type);
            SaveToDatabase(list,type);
            //Console.WriteLine(list.Count);
        }

        public void SaveToDatabase(List<IDictionary<string, string>> list, int type)
        {
            bool isEcho = false;
            string echo = "";
            if (isEcho)
                echo += "Records: " + list.Count() + " in region " + type+"\n";
            
            DateTime dateNow = DateTime.Now;
            int id_reg = 8;
            switch (type)
            {
                case 2:
                    id_reg = 1;
                    break;
            }

            using (var context = new mpvContext())
            {
                if (isEcho)
                    echo += "Connected to database, starting to write...\n";
                foreach (var item in list)
                {
                    // For each depart record
                    string line = item["formatLine"];
                    int route = Helper.getInt(item["route"]);
                    string vehicle = item["vehId"];
                    int delay = Helper.getInt(item["delayMin"]);
                    string from = item["startStat"];
                    string now = item["lastStat"];
                    string to = item["endStat"];
                    bool isFrom = true;
                    bool isNow = true;
                    bool isTo = true;
                    bool needsSave = false;
                    if (from.IsNullOrWhiteSpace())
                        isFrom = false;
                    if (now.IsNullOrWhiteSpace())
                        isNow = false;
                    if (to.IsNullOrWhiteSpace())
                        isTo = false;
                    int vehCount = context.Vehicles.Count(v => v.LongRegNum == vehicle && v.IdReg == id_reg);
                    if (vehCount == 0)
                    {
                        if (isEcho)
                            echo += "-New VEH";
                        Console.WriteLine(Helper.GenerateUuid());
                        try
                        {
                            Vehicle vehObj = new Vehicle()
                            {
                                Id = Helper.GenerateUuid(),
                                LongRegNum = vehicle,
                                AddDate = dateNow,
                                IdReg = id_reg
                            };
                            context.Vehicles.Add(vehObj);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Cannot save vehicle "+vehicle+", skipping line "+line+"/"+route);
                            continue;
                        }

                        needsSave = true;
                    }

                    if (isTo)
                    {
                        int toStCount = context.Stops.Count(s => s.Name == to && s.IdReg == id_reg);
                        if (toStCount == 0 && !to.IsNullOrWhiteSpace())
                        {
                            if (isEcho)
                                echo += "-New To";
                            Stop st = new Stop()
                            {
                                Name = to,
                                IdReg = id_reg
                            };
                            context.Stops.Add(st);
                            needsSave = true;
                        }
                    }

                    if (isFrom)
                    {
                        int fromStCount = context.Stops.Count(s => s.Name == from && s.IdReg == id_reg);
                        if (fromStCount == 0 && !from.IsNullOrWhiteSpace())
                        {
                            if (isEcho)
                                echo += "-New From";
                            Stop st = new Stop()
                            {
                                Name = from,
                                IdReg = id_reg
                            };
                            context.Stops.Add(st);
                            needsSave = true;
                        }
                    }

                    if (isNow)
                    {
                        int nowStCount = context.Stops.Count(s => s.Name == now && s.IdReg == id_reg);
                        if (nowStCount == 0 && !now.IsNullOrWhiteSpace())
                        {
                            if (isEcho)
                                echo += "-New Now";
                            Stop st = new Stop()
                            {
                                Name = now,
                                IdReg = id_reg
                            };
                            context.Stops.Add(st);
                            needsSave = true;
                        }
                    }
                    if(needsSave)
                        context.SaveChanges();

                    Vehicle veh = context.Vehicles.FirstOrDefault(v => v.LongRegNum == vehicle && v.IdReg == id_reg);
                    Stop toStop = null;
                    Stop nowStop = null;
                    Stop fromStop = null;
                    if (!to.IsNullOrWhiteSpace())
                        toStop = context.Stops.FirstOrDefault(s => s.Name == to && s.IdReg == id_reg);
                    if (!now.IsNullOrWhiteSpace())
                        nowStop = context.Stops.FirstOrDefault(s => s.Name == now && s.IdReg == id_reg);
                    if (!from.IsNullOrWhiteSpace())
                        fromStop = context.Stops.FirstOrDefault(s => s.Name == from && s.IdReg == id_reg);

                    if (veh == null || (toStop == null && isTo) || (nowStop == null && isNow) ||
                        (fromStop == null && isFrom))
                    {
                        if (isEcho)
                        {
                            echo += "-ERROR:BadVeh: "+veh==null+", BadTo: "+(toStop == null && isTo)+", BadNow: "+(nowStop == null && isNow)+", BadFrom: "+(fromStop == null && isFrom)+"\n";
                            echo += " > More data: VehId: " + vehicle + ", Line: " + line + ", Route: " + route +
                                    ", Delay: " + delay + ", From: " + from + ", Now: " + now + ", To: " + to + "\n >";
                        }
                        Helper.LogToDatabase("RecordAdder: Probably cannot add record " + line + " (" + route +
                                             ") with vehicle " + vehicle+", from: "+from+", now: "+now+", to: "+to);    
                    }
                    
                    if (veh != null)
                    {
                        veh.LastSeen = DateTime.Now;
                        var depart = context.Depart.FirstOrDefault(d =>
                            d.IdVeh == veh.Id && d.Date.Day == dateNow.Day && d.Date.Month == dateNow.Month && d.Date.Year == dateNow.Year && d.Line == line && d.Route == route);
                        int toId = 1;
                        int fromId = 1;
                        int nowId = 1;
                        if (toStop != null)
                            toId = toStop.Id;
                        if (fromStop != null)
                            fromId = fromStop.Id;
                        if (nowStop != null)
                            nowId = nowStop.Id;
                        if (depart == null)
                        {
                            veh.Departs++;
                            if (isEcho)
                                echo += "-NewDepart";
                            var set = GetVehiclesInSet(veh.Id);
                            Depart dep = new Depart()
                            {
                                IdVeh = veh.Id,
                                Line = line,
                                Route = route,
                                Delay = delay,
                                StartStation = fromId,
                                LastStation = nowId,
                                FinalStation = toId,
                                Date = dateNow.Date,
                                ActDate = dateNow,
                                StartDate = dateNow
                            };

                            if (set != null && set.Count() > 1)
                            {
                                if (isEcho)
                                    echo += "-NewSet:";
                                
                                int pos = 0;
                                foreach (string ve in set)
                                {
                                    pos++;
                                    if (pos == 1)
                                        continue;
                                    if (isEcho)
                                        echo += "+Veh";
                                    Vehicle vehi = context.Vehicles.FirstOrDefault(v => v.Id == ve);
                                    if (vehi != null)
                                    {
                                        vehi.LastSeen = dateNow;
                                        vehi.Departs++;
                                        switch (pos)
                                        {
                                            case 2:
                                                dep.IdVeh2 = ve;
                                                break;
                                            case 3:
                                                dep.IdVeh3 = ve;
                                                break;
                                            default:
                                                Console.Error.WriteLine("Too much long set, depart will ignore longer sets than 3 vehicles.");
                                                break;
                                        }
                                    }
                                }
                            }
                            context.Depart.Add(dep);
                        }
                        else
                        {
                            if (isEcho)
                                echo += "-UpdateDepart";
                            depart.ActDate = dateNow;
                            depart.Delay = delay;
                            depart.FinalStation = toId;
                            depart.LastStation = nowId;

                            var set = GetVehiclesInSet(veh.Id);
                            if (set != null && set.Count() > 1)
                            {
                                if (isEcho)
                                    echo += "-Set:";
                                int pos = 0;
                                foreach (string ve in set)
                                {
                                    pos++;
                                    if (pos == 1)
                                        continue;
                                    Vehicle vehi = context.Vehicles.FirstOrDefault(v => v.Id == ve);
                                    if (isEcho)
                                        echo += "+Veh";
                                    if (vehi != null)
                                    {
                                        vehi.LastSeen = dateNow;
                                    }
                                    else
                                    {
                                        Console.Error.WriteLine("Vehicle ID "+ve+" was not found, but is in set with "+veh.Id);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (isEcho)
                            echo += "-Depart not added!";
                    }
                    if (isEcho)
                        echo += "\n";
                }

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cannot save record to database: "+ex.Message);
                }
            }
            if (isEcho)
                echo += "Exiting...\n";
            if (isEcho)
                Console.WriteLine(echo);
        }

        public List<string> GetVehiclesInSet(string firstVehId)
        {
            List<string> list = new List<string>();
            using (var context = new mpvContext())
            {
                VehSet set = context.VehSets.FirstOrDefault(v => v.IdVeh1 == firstVehId);
                list.Add(firstVehId);
                if (set == null)
                    return list;
                list.Add(set.IdVeh2);
                if(!set.IdVeh3.IsNullOrWhiteSpace())
                    list.Add(set.IdVeh3);
            }
            return list;
        }

        public JsonDocument GetJsonFromSitePid()
        {
            return null;
        }
        public JsonDocument GetJsonFromSiteOdis()
        {
            long millis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            string date = DateTime.Now.ToString("d.M.yyyy");
            
            string url = "http://mpvnet.cz/AXSM/GetViewportObjects?rnd="+millis;

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Content = new StringContent("{\"sid\": \"\", \"s\": 49.54666388055991, \"w\": 17.983357026793552, \"n\": 49.96586312396459, \"e\": 18.74622018597324, \"sOpt\": \"/a/H/h\", \"mppx\": 10, \"mapQuery\": \"ODIS,"+date+" *,all\"}", Encoding.UTF8, "application/json");  
                request.Headers.Add("Accept","application/json, text/javascript, */*; q=0.01");
                request.Headers.Add("Accept-Encoding","gzip, deflate");
                request.Headers.Add("Accept-Language","cs-CZ,cs;q=0.9,en;q=0.8");
                request.Headers.Add("Connection","keep-alive");
                //request.Headers.Add("Content-Length","175");
                //request.Headers.Add("Content-Type","application/json");
                request.Headers.Add("Host","mpvnet.cz");
                request.Headers.Add("Origin","http://mpvnet.cz");
                request.Headers.Add("Referer","http://mpvnet.cz/odis/map");
                request.Headers.Add("User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36");
                request.Headers.Add("X-Requested-With","XMLHttpRequest");

                HttpResponseMessage res = client.SendAsync(request).Result;
                Stream receiveStream = res.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
                string jsonString = readStream.ReadToEnd();

                try
                {
                    var json = JsonDocument.Parse(jsonString);
                    return json;
                }
                catch (Exception e)
                {
                    Helper.LogToDatabase("RecordAdder Error: Cannot convert string to JSON. [" + e.Message + "]");
                }

                //Console.WriteLine(jsonString);
                return null;
            }
            return null;
        }

        public List<IDictionary<string, string>> CreateList(JsonDocument json, int type)
        {
            List<IDictionary<string, string>> res = new List<IDictionary<string, string>>();
            if (json == null)
                return res;
            var list = json.RootElement.GetProperty("T");

            try
            {
                foreach (var element in list.EnumerateArray())
                {
                    try
                    {
                        var text = element.GetProperty("cn").ToString();
                        var del = element.GetProperty("ds").ToString();
                        del = del.Replace("~", " ");
                        del = del.Replace("$z", "");
                        del = del.Replace("p", "");
                        del = del.Replace("n", "0");
                        del = del.Split(new string[] {" min"}, StringSplitOptions.None)[0];
                        //Console.WriteLine(text);
                        string[] data = text.Split('|');

                        // 912448|448|212|Ostrava,Zábřeh,Výškovická|270|Ostrava,Hrabůvka,Poliklinika|270|984|Ostrava,Zábřeh,AVION|Havířov,Podlesí,aut.nádr.|1|421004|0|0
                        if (data.Length == 14)
                        {
                            string lastStat;
                            int delInSec;
                            int delInMin = 0;
                            string nextStat;
                            string nextDepartInMin;
                            if (data[3] != "")
                            {
                                lastStat = data[3];
                                delInSec = int.Parse(data[4]);
                                delInMin = delInSec / 60;
                            }
                            else
                            {
                                lastStat = "";
                                delInSec = 0;
                                delInMin = 0;
                            }

                            if (data[5] != "")
                            {
                                nextStat = data[5];
                                nextDepartInMin = data[7];
                            }
                            else
                            {
                                nextStat = "";
                                nextDepartInMin = "0";
                            }

                            string lLine = data[0];
                            string formattedLine = data[1];
                            string route = data[2];
                            string startStat = data[8];
                            string endStop = data[9];
                            string vehId = data[11];

                            bool valid = false;
                            switch (type)
                            {
                                case 1:
                                    valid = IsValidOdis(formattedLine, vehId);
                                    break;
                                case 2:
                                    valid = IsValidPid(formattedLine, vehId);
                                    break;
                            }

                            if (valid)
                            {
                                IDictionary<string, string> arr = new Dictionary<string, string>()
                                {
                                    {"formatLine", formattedLine},
                                    {"vehId", vehId},
                                    {"longLine", lLine},
                                    {"route", route},
                                    {"lastStat", lastStat},
                                    {"delaySec", delInSec.ToString()},
                                    {"delayMin", delInMin.ToString()},
                                    {"nextStat", nextStat},
                                    {"nextDepartMin", nextDepartInMin},
                                    {"startStat", startStat},
                                    {"endStat", endStop}
                                };
                                res.Add(arr);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error creating list: " + ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating list: " + e.Message);
            }

            return res;
        }

        private bool IsValidOdis(string line, string vehicle)
        {
            bool loadDpo = true;
            bool loadMhdFM = false;
            bool loadMhdKar = false;
            bool loadMhdHav = true;
            bool loadPhdFmVeh = false;
            bool loadPhdFrVeh = false;
            bool loadPhdCtVeh = false;
            bool loadPhdHav1Veh = true;
            bool loadPhdHav2Veh = true;
            bool loadMhdHavVeh = true;
            bool loadMhdKarVeh = false;
            bool loadMhdFmVeh = false;
            bool loadPhdOrlVeh = false;
            bool loadPhdKarVeh = false;
            bool loadPhdHluVeh = false;
            bool loadPhdHav = true;
            
            string[] allowedLin = {"366","365","533","369","368","453"};
            string[] allowedVeh = {};

            int l = -1; 
            try { l = int.Parse(line); }
            catch (Exception e) {}
            
            int v = -1; 
            try { v = int.Parse(vehicle); }
            catch (Exception e) {}

            if (allowedLin.Contains(line) || allowedVeh.Contains(vehicle))
                return true;
            
            if ((l < 130 && l > 0 && loadDpo) || (l >= 400 && l < 440 && loadMhdHav) || (l >= 500 && l <= 520 && loadMhdKar) || 
                (v > 0 && v < 900 && loadMhdFmVeh) || (l >= 440 && l < 500 && loadPhdHav) ||
                (vehicle.StartsWith("33") && loadPhdFmVeh) || (vehicle.StartsWith("32") && loadPhdFrVeh) ||
                (vehicle.StartsWith("73") && loadPhdCtVeh) || (vehicle.StartsWith("50") && loadMhdKarVeh) ||
                (vehicle.StartsWith("42") && loadPhdHav1Veh) || (vehicle.StartsWith("43") && loadPhdHav2Veh) ||
                (vehicle.StartsWith("41") && loadMhdHavVeh) || (vehicle.StartsWith("51") && loadPhdOrlVeh) ||
                (vehicle.StartsWith("52") && loadPhdKarVeh) || (vehicle.StartsWith("24") && loadPhdHluVeh) ||
                (l == -1 && loadDpo && !line.StartsWith("Os") && !line.StartsWith("Sp")))
            {
                return true;
            }
            return false;
        }
        
        private bool IsValidPid(string line, string vehicle)
        {
            bool loadTrams = true;
            bool loadBuses = true;
            bool loadPhdBuses = true;
            bool loadTrolley = true;
            bool loadListedVeh = true;
            
            string[] allowedLin = {"366","365","533","369","368","453"};
            string[] allowedVeh = {};

            int l = -1; 
            try { l = int.Parse(line); }
            catch (Exception e) {}
            
            int v = -1; 
            try { v = int.Parse(vehicle); }
            catch (Exception e) {}

            if (allowedLin.Contains(line) || allowedVeh.Contains(vehicle))
                return true;
            
            if ((l < 50 && l > 0 && loadTrams) || (l >= 100 && l <= 250 && loadBuses) || 
                (l >= 900 && l <= 950) || (l >= 300 && l < 800 && loadPhdBuses) || 
                (l >= 950 && l < 1000 && loadPhdBuses) || (l >= 50 && l <=100 && loadTrolley) ||
                (l == -1 && !line.StartsWith("Os") && !line.StartsWith("Sp") && !line.StartsWith("R") && 
                 !line.StartsWith("U") && !line.StartsWith("L") && !line.StartsWith("S")))
            {
                return true;
            }
            return false;
        }
    }
}