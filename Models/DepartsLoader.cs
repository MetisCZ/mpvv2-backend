using System;
using System.Collections.Generic;
using System.Linq;
using mpvv2.DbModels;
using mpvv2.Models.DBModels;

namespace mpvv2.Models
{
    public class DepartsLoader
    {

        public string GetJsonFromDepartsList(List<Dictionary<string, object>> list)
        {
            string res = "[";
            int i = 0;
            foreach (Dictionary<string,object> item in list)
            {
                if (i != 0)
                    res += ",";
                i++;

                string moreVehs = "[";
                if (item.ContainsKey("more_vehs"))
                {
                    List<Dictionary<string, object>> l = (List<Dictionary<string, object>>) item["more_vehs"];
                    bool firstItem = true;
                    foreach (var it in l)
                    {
                        if (firstItem)
                            firstItem = false;
                        else
                            moreVehs += ",";
                        moreVehs += "{\"id\":\"" + it["id"] + "\", \"reg_num\": \""+it["reg_num"]+"\"}";
                    }
                    
                    
                }
                moreVehs += "]";
                
                string manufYear = ModelsManager.getFormattedCustomDate(item["manufac_year"].ToString());
                string ac = ModelsManager.getAirConditionAsStringStatic(Helper.getInt(item["air_condition"].ToString()));
                string actDate = ModelsManager.getFormattedDateTimeNoSecs(DateTime.Parse(item["a_date"].ToString()));
                string startDate = ModelsManager.getFormattedDateTimeNoSecs(DateTime.Parse(item["s_date"].ToString()));
                string date = DateTime.Parse(item["date"].ToString()).ToString("yyyy-MM-dd");
                res += "{\"line\": \"" + item.GetValue("line") + "\", \"reg_number\": \""+item.GetValue("reg_num")+"\", \"long_reg_num\": \""+item.GetValue("long_reg_num")+"\", "+
                       "\"carrier\": \""+item.GetValue("carrier_name")+"\", \"final_station\":\""+item.GetValue("final_station")+"\", \"delay\":\""+item.GetValue("delay")+"\", "+
                       "\"start_station\":\""+item.GetValue("start_station")+"\", \"last_station\":\""+item.GetValue("last_station")+"\", \"route\":\""+item.GetValue("route")+"\", "+
                       "\"a_date\":\""+actDate+"\", \"s_date\":\""+startDate+"\", \"v_manuf\":\""+item.GetValue("manufacturer_name")+"\", "+
                       "\"v_type\":\""+item.GetValue("vehicle_type")+"\", \"v_manuf_year\":\""+manufYear+"\", \"air_condition\":\""+ac+"\", "+
                       "\"was_in\":\""+item.GetValue("was_in")+"\", \"date\":\""+date+"\", \"id\":\""+item.GetValue("id")+"\", \"more_vehs\":"+moreVehs+"}";
            }

            res += "]";
            return res;
        }
        
        public List<Dictionary<string, object>> LoadDeparts(string line=null, string vehId=null, string day=null, string fromDate=null, string toDate=null, string order=null, bool isDesc=false, int limit=1000, string carrier=null, string vehType=null, string manuf=null, string route=null)
        {
            // depart        - d
            // carrier       - c
            // stop          - s
            // vehicle       - v
            // vehicle_manuf - m
            // vehicle_type  - t
            string desc = "ASC";
            if (order.IsNullOrWhiteSpace())
                order = "act_date";
            if (isDesc)
                desc = "DESC";
            if (limit < 1 || limit > 10000)
                limit = 1000;
            string whereQuery = "";
            List<string> ifs = new List<string>();
            if(!line.IsNullOrWhiteSpace())
                ifs.Add("d.line = @line");
            if(!vehId.IsNullOrWhiteSpace())
                ifs.Add("v.long_reg_num = @vehid");
            if(!day.IsNullOrWhiteSpace())
                ifs.Add("DATE(d.act_date) = DATE(@day)");
            if(!fromDate.IsNullOrWhiteSpace())
                ifs.Add("DATE(d.act_date) >= DATE(@fromdate)");
            if(!toDate.IsNullOrWhiteSpace())
                ifs.Add("DATE(d.act_date) <= DATE(@todate)");
            if (!carrier.IsNullOrWhiteSpace())
                ifs.Add("c.name LIKE @carrier");
            if (!vehType.IsNullOrWhiteSpace())
                ifs.Add("u.name LIKE @vehtype");
            if (!manuf.IsNullOrWhiteSpace())
                ifs.Add("m.name LIKE @manuf");
            if(!route.IsNullOrWhiteSpace())
                ifs.Add("d.route = @route");
            if (ifs.Count() > 0)
                whereQuery += "WHERE ";
            int i = 0;
            foreach (var iff in ifs)
            {
                if (i > 0)
                    whereQuery += " AND ";
                whereQuery += iff;
                i++;
            }

            string query =
                "SELECT d.id_veh2,d.id_veh3,v.id,v.long_reg_num, v.reg_num,d.date,c.name AS carrier_name,m.name AS manufacturer_name,u.name AS vehicle_type, "+
                "v.manufac_year,v.air_condition, d.line, d.start_date AS s_date, (SELECT s.name FROM stop s WHERE s.id = d.final_station) AS final_station,"+
                "d.delay,(SELECT s.name FROM stop s WHERE s.id = d.last_station) AS last_station,"+
                "(SELECT s.name FROM stop s WHERE s.id = d.start_station) AS start_station, d.route, v.id AS id_veh, d.was_in,d.act_date AS a_date"+
                " FROM depart d INNER JOIN vehicle v ON (v.id = d.id_veh) LEFT JOIN veh_up_type u ON (u.id = v.id_vut)"+
                " INNER JOIN veh_type t ON(t.id=u.id_vet) INNER JOIN manufacturer m ON (m.id = t.id_man) LEFT JOIN depot e ON (e.id = v.id_dep)"+
                " INNER JOIN carrier c ON (e.id_car = c.id)"+
                whereQuery+
                " ORDER BY "+order+" "+desc+
                " LIMIT @limit;";
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("@line", line);
            parameters.Add("@vehid", vehId);
            parameters.Add("@day", day);
            parameters.Add("@fromdate", fromDate);
            parameters.Add("@todate", toDate);
            parameters.Add("@carrier", carrier);
            parameters.Add("@vehtype", vehType);
            parameters.Add("@manuf", manuf);
            parameters.Add("@route", route);
            parameters.Add("@order", order);
            parameters.Add("@limit", limit);
            List<Dictionary<string, object>> res = null;
            using (var db = new Database())
            {
                res = db.Select(query,parameters);
            }

            mpvContext context = null;
            foreach (var dep in res)
            {
                if (dep.ContainsKey("id_veh2"))
                {
                    if (context == null)
                        context = new mpvContext();
                    
                    string idVeh2 = dep["id_veh2"].ToString();
                    Vehicle vehicle = context.Vehicles.FirstOrDefault(v => v.Id == idVeh2);
                    Dictionary<string, object> data = new Dictionary<string, object>
                    {
                        {"id", vehicle.Id},
                        {"reg_num", vehicle.RegNum}
                    };
                    dep["more_vehs"] = new List<Dictionary<string, object>>();
                    ((List<Dictionary<string, object>>) dep["more_vehs"]).Add(data);
                }
                if (dep.ContainsKey("id_veh3"))
                {
                    string idVeh3 = dep["id_veh3"].ToString();
                    Vehicle vehicle = context.Vehicles.FirstOrDefault(v => v.Id == idVeh3);
                    Dictionary<string, object> data = new Dictionary<string, object>
                    {
                        {"id", vehicle.Id},
                        {"reg_num", vehicle.RegNum}
                    };
                    ((List<Dictionary<string, object>>) dep["more_vehs"]).Add(data);
                }
            }
            context?.Dispose();

            //Console.WriteLine(res.Count());
            return res;
        }
    }
}