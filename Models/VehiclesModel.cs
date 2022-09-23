using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using mpvv2.Controllers;
using mpvv2.DbModels;
using mpvv2.Models.DBModels;

namespace mpvv2.Models
{
    public class VehiclesModel
    {
        public List<Dictionary<string, object>> GetNowActive(string line)
        {
            var ra = new RecordAdder();
            JsonDocument json = ra.GetJsonFromSiteOdis();
            var data = ra.CreateList(json, 1);
            ra.SaveToDatabase(data,1);

            bool firstVeh = true;
            string vehRegNums = "";
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            int i = 0;
            foreach (var vehicle in data)
            {
                if (line == null || vehicle.GetValue("formatLine")?.ToString() == line)
                {
                    i++;

                    if (!firstVeh)
                        vehRegNums += ",";
                    vehRegNums += "\"" + vehicle.GetValue("vehId") + "\"";
                    firstVeh = false;

                    Dictionary<string, object> nowAct = new Dictionary<string, object>();
                    nowAct.Add("longRegNum",vehicle.GetValue("vehId").ToString());
                    nowAct.Add("lastStation",vehicle.GetValue("lastStat")?.ToString());
                    nowAct.Add("finalStation",vehicle.GetValue("endStat")?.ToString());
                    nowAct.Add("line",vehicle.GetValue("formatLine")?.ToString());
                    nowAct.Add("route",vehicle.GetValue("route")?.ToString());
                    nowAct.Add("delay",vehicle.GetValue("delay","0")?.ToString());
                    list.Add(nowAct);
                }
            }

            if (vehRegNums == "")
                return list;

            string query =
                "SELECT v.part, v.long_reg_num, v.reg_num,v.manufac_year,m.name AS manufacturer_name, " +
                "u.name AS vehicle_type,v.air_condition" +
                " FROM vehicle v LEFT JOIN veh_up_type u ON (u.id = v.id_vut)" +
                " INNER JOIN veh_type t ON(t.id=u.id_vet) INNER JOIN manufacturer m ON (m.id = t.id_man)" +
                " WHERE v.long_reg_num IN (" + vehRegNums + ")" +
                " ORDER BY long_reg_num ASC;";
            //Console.WriteLine(query);
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();

            List<Dictionary<string, object>> res = null;
            using (var db = new Database())
            {
                res = db.Select(query,parameters);   
            }

            foreach (var vehicle in res)
            {
                var veh = list.FirstOrDefault(v =>
                    v.GetValue("longRegNum")?.ToString() == vehicle.GetValue("long_reg_num").ToString());
                if(veh == null)
                    continue;
                string manufYear = ModelsManager.getFormattedCustomDate(vehicle["manufac_year"]?.ToString());
                string ac = ModelsManager.getAirConditionAsStringStatic(Helper.getInt(vehicle["air_condition"]?.ToString()));
                veh.Add("part",vehicle.GetValue("part")?.ToString());
                veh.Add("manufacturedDate",manufYear);
                veh.Add("airCondition",ac);
                veh.Add("vehicleType",vehicle["vehicle_type"]?.ToString());
                veh.Add("manufacturerName",vehicle["manufacturer_name"]?.ToString());
                veh.Add("regNum",vehicle["reg_num"]?.ToString());
            }

            return list;
        }
 
        public Dictionary<string, object> GetVehicleData(Api2Controller.VehicleData data)
        {
            var vehicleInfo = GetVehicleInfo(data.id);

            /*foreach (var vehicle in vehicleInfo)
            {

                actVeh.Add("departs",new List<Dictionary<string, object>>());
                var departInfo = GetVehicleDeparts(vehicle["id"].ToString(), offset, offsetCount, whenDate);
                if(departInfo.Count > 0)
                    actVeh["departs"] = departInfo;
                
            }*/
            return vehicleInfo;
        }
        
        private Dictionary<string, object> GetVehicleInfo(string vehId)
        {
            
            Dictionary<string, object> list = new Dictionary<string, object>();
            
            string query =
                "SELECT v.departs, v.id, v.part, v.cond, UNIX_TIMESTAMP(v.last_update) AS last_update, v.spz, v.serial_number, v.vin, v.low_floor, v.info_panel, v.long_reg_num, v.departs, v.mini_picture, v.reg_num,c.name AS carrier_name,v.manufac_year, " +
                "v.air_condition,UNIX_TIMESTAMP(v.last_seen) AS last_seen, u.name AS vehUpName, u.id AS vehUpId, t.name AS vehTypeName, t.id AS vehTypeId, "+
                "m.name AS manufacturerName, m.id AS manufacturerId, e.id AS depotId, e.name AS depotName, c.id AS carrierId, c.name AS carrierName"+
                " FROM vehicle v LEFT JOIN veh_up_type u ON (u.id = v.id_vut)" +
                " INNER JOIN veh_type t ON(t.id=u.id_vet) INNER JOIN manufacturer m ON (m.id = t.id_man)" +
                " LEFT JOIN depot e ON (e.id = v.id_dep) INNER JOIN carrier c ON (e.id_car = c.id)"+
                " WHERE v.id = @id"+
                " LIMIT 2;";
            //Console.WriteLine(query);
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("@id", vehId);
            
            List<Dictionary<string, object>> res = null;
            using (var db = new Database())
            {
                res = db.Select(query,parameters);   
            }

            if (res.Count != 1)
                return list;
            
            var vehicle = res[0];

            { // Veh set
                string sql =
                    "SELECT id_veh1, id_veh2, id_veh3, date_from, (SELECT reg_num FROM vehicle v WHERE v.id = id_veh1) AS reg_num1, " +
                    "(SELECT reg_num FROM vehicle v WHERE v.id = id_veh2) AS reg_num2, (SELECT reg_num FROM vehicle v WHERE v.id = id_veh3) AS reg_num3" +
                    " FROM veh_set" +
                    " WHERE id_veh1 = @id OR id_veh2 = @id OR id_veh3 = @id";
                Dictionary<string, dynamic> prms = new Dictionary<string, dynamic>();
                prms.Add("@id", vehId);

                res = null;
                using (var db = new Database())
                {
                    res = db.Select(sql, prms);
                }

                if (res.Count == 1)
                {
                    List<Dictionary<string, object>> actSet = new List<Dictionary<string, object>>();
                    /*Dictionary<string, object> set = new Dictionary<string, object>();
                    set.Add("id", res[0].GetValue("id_veh1").ToString());
                    set.Add("regNum", res[0].GetValue("reg_num1").ToString());
                    actSet.Add(set);*/
                    
                    Dictionary<string, object> set2 = new Dictionary<string, object>();
                    set2.Add("id", res[0].GetValue("id_veh2").ToString());
                    set2.Add("regNum", res[0].GetValue("reg_num2").ToString());
                    actSet.Add(set2);

                    if (res[0].GetValue("reg_num3", null) != null)
                    {
                        Dictionary<string, object> set3 = new Dictionary<string, object>();
                        set3.Add("id", res[0].GetValue("id_veh3").ToString());
                        set3.Add("regNum", res[0].GetValue("reg_num3").ToString());
                        actSet.Add(set3);
                    }

                    list.Add("sets", actSet);
                }
            }
            { // Veh set history
                
            }

            string manufYear = ModelsManager.getFormattedCustomDate(vehicle["manufac_year"]?.ToString());

            list.Add("id",vehicle["id"].ToString());
            list.Add("longRegNum",vehicle["long_reg_num"]?.ToString());
            list.Add("regNum",vehicle["reg_num"]?.ToString());
            list.Add("cond",vehicle.GetValue("cond")?.ToString());
            list.Add("lastUpdate",vehicle.GetValue("last_update")?.ToString());
            list.Add("SPZ",vehicle.GetValue("spz")?.ToString());
            list.Add("serialNumber",vehicle.GetValue("serial_number")?.ToString());
            list.Add("vin",vehicle.GetValue("vin")?.ToString());
            list.Add("lowFloor",vehicle.GetValue("low_floor")?.ToString());
            list.Add("infoPanel",vehicle.GetValue("info_panel")?.ToString());
            list.Add("part",vehicle["part"]?.ToString());
            list.Add("manufacturedDate",manufYear);
            list.Add("manufacturerName",vehicle["manufacturerName"]?.ToString());
            list.Add("manufacturerId",vehicle["manufacturerId"]?.ToString());
            list.Add("vehicleUpType",vehicle["vehUpName"]?.ToString());
            list.Add("vehicleUpTypeId",vehicle["vehUpId"]?.ToString());
            list.Add("vehicleTypeName",vehicle["vehTypeName"]?.ToString());
            list.Add("vehicleTypeId",vehicle["vehTypeId"]?.ToString());
            list.Add("depotName",vehicle["depotName"]?.ToString());
            list.Add("depotId",vehicle["depotId"]?.ToString());
            list.Add("carrierName",vehicle["carrierName"]?.ToString());
            list.Add("carrierId",vehicle["carrierId"]?.ToString());
            list.Add("thumbnail",vehicle.GetValue("mini_picture")?.ToString());
            list.Add("airCondition",vehicle.GetValue("air_condition")?.ToString());
            list.Add("lastSeen",vehicle.GetValue("last_seen")?.ToString());
            list.Add("departsCount",vehicle.GetValue("departs")?.ToString());

            return list;
        }
        
        private List<Dictionary<string, object>> GetVehicleDeparts(string idVehicle, string offset, string offsetCount, DateTime whenDate)
        {
            List < Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            int offsetInt = int.Parse(offset);
            int offsetCountInt = int.Parse(offsetCount);
            if (offsetCountInt > 100)
                offsetCountInt = 10;
            if (offsetInt > 2000)
                offsetInt = 0;
            string query =
                "SELECT d.id_veh2,d.id_veh3,d.date, "+
                " d.line, d.start_date AS s_date, (SELECT s.name FROM stop s WHERE s.id = d.final_station) AS final_station,"+
                " d.delay,(SELECT s.name FROM stop s WHERE s.id = d.last_station) AS last_station,"+
                " (SELECT s.name FROM stop s WHERE s.id = d.start_station) AS start_station, d.route, d.id_veh AS id_veh,d.act_date AS a_date"+
                " FROM depart_odis d"+
                " WHERE id_veh = @idVehicle AND date = @date"+
                " ORDER BY act_date DESC"+
                " LIMIT @offsetInt, @offsetCountInt;";
            //Console.WriteLine(query);
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("@idVehicle", idVehicle);
            parameters.Add("@date", whenDate);
            parameters.Add("@offsetInt", offsetInt);
            parameters.Add("@offsetCountInt", offsetCountInt);
            
            List<Dictionary<string, object>> res = null;
            using (var db = new Database())
            {
                res = db.Select(query,parameters);
            }
            
            mpvContext context = null;
            foreach (var dep in res)
            {
                string veh2LongNumber = null;
                string veh2Number = null;
                string veh3LongNumber = null;
                string veh3Number = null;
                
                if (dep.ContainsKey("id_veh2"))
                {
                    if (context == null)
                        context = new mpvContext();
                    
                    string idVeh2 = dep["id_veh2"].ToString();
                    Vehicle vehicle = context.Vehicles.FirstOrDefault(v => v.Id == idVeh2);
                    veh2LongNumber = vehicle.LongRegNum;
                    veh2Number = vehicle.RegNum;
                }
                if (dep.ContainsKey("id_veh3"))
                {
                    string idVeh3 = dep["id_veh3"].ToString();
                    Vehicle vehicle = context.Vehicles.FirstOrDefault(v => v.Id == idVeh3);
                    veh3LongNumber = vehicle.LongRegNum;
                    veh3Number = vehicle.RegNum;
                }
                string actDate = ModelsManager.getFormattedDateTimeNoSecs(DateTime.Parse(dep["a_date"].ToString()));
                string startDate = ModelsManager.getFormattedDateTimeNoSecs(DateTime.Parse(dep["s_date"].ToString()));
                
                Dictionary<string, object> actVeh = new Dictionary<string, object>();
                actVeh.Add("lastDate",actDate);
                actVeh.Add("firstDate",startDate);
                if (veh2LongNumber != null)
                {
                    actVeh.Add("veh2LongRegNum", veh2LongNumber);
                    actVeh.Add("veh2RegNum", veh2Number);
                }

                if (veh3LongNumber != null)
                {
                    actVeh.Add("veh3LongRegNum", veh3LongNumber);
                    actVeh.Add("veh3RegNum", veh3Number);
                }
                actVeh.Add("line",dep["line"]?.ToString());
                actVeh.Add("route",dep["route"]?.ToString());
                actVeh.Add("finalStation",dep["final_station"]?.ToString());
                actVeh.Add("lastStation",dep["last_station"]?.ToString());
                actVeh.Add("firstStation",dep["start_station"]?.ToString());
                actVeh.Add("delay",dep["delay"]?.ToString());
                list.Add(actVeh);
            }
            context?.Dispose();

            return list;
        }

        public List<Dictionary<string, object>> GetVehiclesList(Api2Controller.SearchData data)
        {
            List < Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            string whereQuery = " ";
            bool firstIf = true;
            if (!string.IsNullOrEmpty(data.line))
            {
                if (firstIf)
                    whereQuery += "WHERE ";
                else
                    whereQuery += "AND ";
                whereQuery += "line = @line ";
                firstIf = false;
            }
            if (!string.IsNullOrEmpty(data.route))
            {
                if (firstIf)
                    whereQuery += "WHERE ";
                else
                    whereQuery += "AND ";
                whereQuery += "route = @route ";
                firstIf = false;
            }
            if (!string.IsNullOrEmpty(data.vehicle))
            {
                if (firstIf)
                    whereQuery += "WHERE ";
                else
                    whereQuery += "AND ";
                whereQuery += "reg_num = @vehicle OR long_reg_num = @vehicle ";
                firstIf = false;
            }
            if (!string.IsNullOrEmpty(data.part))
            {
                if (firstIf)
                    whereQuery += "WHERE ";
                else
                    whereQuery += "AND ";
                string[] parts = data.part.Split(',');
                whereQuery += "part IN (";
                bool firstItem = true;
                foreach (string part in parts)
                {
                    if (!int.TryParse(part, out _))
                        return list;
                    if (!firstItem)
                        whereQuery += ",";
                    whereQuery += part;
                    firstItem = false;
                }
                whereQuery += ") ";
                firstIf = false;
            }
            if (!string.IsNullOrEmpty(data.fromDate))
            {
                if (firstIf)
                    whereQuery += "WHERE ";
                else
                    whereQuery += "AND ";
                whereQuery += "DATE(d.act_date) >= DATE(@fromDate) ";
                firstIf = false;
            }
            if (!string.IsNullOrEmpty(data.toDate))
            {
                if (firstIf)
                    whereQuery += "WHERE ";
                else
                    whereQuery += "AND ";
                whereQuery += "DATE(d.act_date) <= DATE(@toDate) ";
                firstIf = false;
            }

            int limit = data.limit;
            if (data.limit > 2000 || data.limit < 1)
                limit = 100;
            
            string query =
                "SELECT COUNT(v.id) AS departs, v.id, v.part, v.long_reg_num, v.reg_num,c.name AS carrier_name,v.manufac_year,m.name AS manufacturer_name, " +
                "u.name AS vehicle_type,v.air_condition,UNIX_TIMESTAMP(v.last_seen) AS last_seen"+
                " FROM vehicle v LEFT JOIN depart_odis d ON (v.id = d.id_veh) LEFT JOIN veh_up_type u ON (u.id = v.id_vut)" +
                " INNER JOIN veh_type t ON(t.id=u.id_vet) INNER JOIN manufacturer m ON (m.id = t.id_man)" +
                " LEFT JOIN depot e ON (e.id = v.id_dep) INNER JOIN carrier c ON (e.id_car = c.id)"+
                whereQuery+
                " GROUP BY v.id"+
                " ORDER BY id_car, long_reg_num ASC"+
                " LIMIT @limit;";
            //Console.WriteLine(query);
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("@line", data.line);
            parameters.Add("@route", data.route);
            parameters.Add("@part",data.part);
            parameters.Add("@vehicle",data.vehicle);
            parameters.Add("@fromDate",data.fromDate);
            parameters.Add("@toDate",data.toDate);
            parameters.Add("@limit",limit);

            List<Dictionary<string, object>> res = null;
            using (var db = new Database())
            {
                res = db.Select(query,parameters);
            }
            
            foreach (var vehicle in res)
            {
                Dictionary<string, object> actVeh = new Dictionary<string, object>();
                string manufYear = ModelsManager.getFormattedCustomDate(vehicle["manufac_year"]?.ToString());
                actVeh.Add("id",vehicle["id"].ToString());
                actVeh.Add("longRegNum",vehicle["long_reg_num"]?.ToString());
                actVeh.Add("departsCount",vehicle["departs"]?.ToString());
                actVeh.Add("regNum",vehicle["reg_num"]?.ToString());
                actVeh.Add("part",vehicle["part"]?.ToString());
                actVeh.Add("carrierName",vehicle["carrier_name"]?.ToString());
                actVeh.Add("manufacturedDate",manufYear);
                actVeh.Add("manufacturerName",vehicle["manufacturer_name"]?.ToString());
                actVeh.Add("vehicleType",vehicle["vehicle_type"]?.ToString());
                actVeh.Add("airCondition",vehicle["air_condition"]?.ToString());
                actVeh.Add("lastSeen",vehicle.GetValue("last_seen"));
                list.Add(actVeh);
            }

            return list;
        }
        
        public List<Dictionary<string, object>> GetDepartsList(Api2Controller.SearchData data)
        {
            List < Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            
            string whereQuery = " ";
            bool firstIf = true;
            if (!string.IsNullOrEmpty(data.line))
            {
                if (firstIf)
                    whereQuery += "WHERE ";
                else
                    whereQuery += "AND ";
                whereQuery += "line = @line ";
                firstIf = false;
            }
            if (!string.IsNullOrEmpty(data.route))
            {
                if (firstIf)
                    whereQuery += "WHERE ";
                else
                    whereQuery += "AND ";
                whereQuery += "route = @route ";
                firstIf = false;
            }
            if (!string.IsNullOrEmpty(data.vehicle))
            {
                if (firstIf)
                    whereQuery += "WHERE ";
                else
                    whereQuery += "AND ";
                whereQuery += "reg_num = @vehicle OR long_reg_num = @vehicle ";
                firstIf = false;
            }
            if (!string.IsNullOrEmpty(data.part))
            {
                if (firstIf)
                    whereQuery += "WHERE ";
                else
                    whereQuery += "AND ";
                string[] parts = data.part.Split(',');
                whereQuery += "part IN (";
                bool firstItem = true;
                foreach (string part in parts)
                {
                    if (!int.TryParse(part, out _))
                        return list;
                    if (!firstItem)
                        whereQuery += ",";
                    whereQuery += part;
                    firstItem = false;
                }
                whereQuery += ") ";
                firstIf = false;
            }
            if (!string.IsNullOrEmpty(data.fromDate))
            {
                if (firstIf)
                    whereQuery += "WHERE ";
                else
                    whereQuery += "AND ";
                whereQuery += "DATE(d.act_date) >= DATE(@fromDate) ";
                firstIf = false;
            }
            if (!string.IsNullOrEmpty(data.toDate))
            {
                if (firstIf)
                    whereQuery += "WHERE ";
                else
                    whereQuery += "AND ";
                whereQuery += "DATE(d.act_date) <= DATE(@toDate) ";
                firstIf = false;
            }

            int limit = data.limit;
            if (data.limit > 2000 || data.limit < 1)
                limit = 100;

            string query =
                "SELECT d.id AS idDepart, d.id_veh2,d.id_veh3,v.id AS id_veh,v.long_reg_num, v.reg_num,m.name AS manufacturer_name,u.name AS vehicle_type, "+
                "v.manufac_year,v.air_condition, d.date, d.line, UNIX_TIMESTAMP(d.start_date) AS s_date, (SELECT s.name FROM stop s WHERE s.id = d.final_station) AS final_station,"+
                "d.delay,(SELECT s.name FROM stop s WHERE s.id = d.last_station) AS last_station,"+
                "(SELECT s.name FROM stop s WHERE s.id = d.start_station) AS start_station, d.route, v.id AS id_veh, UNIX_TIMESTAMP(d.act_date) AS a_date"+
                " FROM depart_odis d INNER JOIN vehicle v ON (v.id = d.id_veh) LEFT JOIN veh_up_type u ON (u.id = v.id_vut)"+
                " INNER JOIN veh_type t ON(t.id=u.id_vet) INNER JOIN manufacturer m ON (m.id = t.id_man) LEFT JOIN depot e ON (e.id = v.id_dep) INNER JOIN carrier c ON (e.id_car = c.id)"+
                whereQuery+
                " ORDER BY act_date DESC, start_date DESC"+
                " LIMIT @limit;";
            //Console.WriteLine(query);
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("@line", data.line);
            parameters.Add("@route", data.route);
            parameters.Add("@part",data.part);
            parameters.Add("@vehicle",data.vehicle);
            parameters.Add("@fromDate",data.fromDate);
            parameters.Add("@toDate",data.toDate);
            parameters.Add("@limit",limit);
            
            List<Dictionary<string, object>> res = null;
            using (var db = new Database())
            {
                res = db.Select(query,parameters);
            }
            
            mpvContext context = null;
            foreach (var dep in res)
            {
                Dictionary<string, object> actVeh = new Dictionary<string, object>();
                List<Dictionary<string, object>> sets = new List<Dictionary<string, object>>();

                if (dep.ContainsKey("id_veh2"))
                {
                    if (context == null)
                        context = new mpvContext();
                    
                    string idVeh2 = dep["id_veh2"].ToString();
                    Vehicle vehicle = context.Vehicles.FirstOrDefault(v => v.Id == idVeh2);
                    
                    Dictionary<string, object> actSet = new Dictionary<string, object>();
                    actSet.Add("id",vehicle.Id);
                    actSet.Add("longRegNum",vehicle.LongRegNum);
                    actSet.Add("regNum",vehicle.RegNum);
                    
                    if (vehicle.LongRegNum != null)
                        sets.Add(actSet);
                }
                if (dep.ContainsKey("id_veh3"))
                {
                    string idVeh3 = dep["id_veh3"].ToString();
                    Vehicle vehicle = context.Vehicles.FirstOrDefault(v => v.Id == idVeh3);
                    
                    Dictionary<string, object> actSet = new Dictionary<string, object>();
                    actSet.Add("id",vehicle.Id);
                    actSet.Add("longRegNum",vehicle.LongRegNum);
                    actSet.Add("regNum",vehicle.RegNum);
                    
                    if (vehicle.LongRegNum != null)
                        sets.Add(actSet);
                }

                
                actVeh.Add("lastDate",dep["a_date"].ToString());
                actVeh.Add("firstDate",dep["s_date"].ToString());
                actVeh.Add("sets",sets);
                actVeh.Add("id",dep["idDepart"]?.ToString());
                actVeh.Add("line",dep["line"]?.ToString());
                actVeh.Add("route",dep["route"]?.ToString());
                actVeh.Add("finalStation",dep["final_station"]?.ToString());
                actVeh.Add("lastStation",dep["last_station"]?.ToString());
                actVeh.Add("firstStation",dep.GetValue("start_station")?.ToString());
                actVeh.Add("delay",dep["delay"]?.ToString());

                string manufYear = ModelsManager.getFormattedCustomDate(dep["manufac_year"]?.ToString());
                actVeh.Add("vehId",dep["id_veh"]?.ToString());
                actVeh.Add("longRegNum",dep["long_reg_num"]?.ToString());
                actVeh.Add("regNum",dep["reg_num"]?.ToString());
                actVeh.Add("manufacturedDate",manufYear);
                actVeh.Add("manufacturerName",dep["manufacturer_name"]?.ToString());
                actVeh.Add("vehicleType",dep["vehicle_type"]?.ToString());
                actVeh.Add("airCondition",dep["air_condition"]?.ToString());
                list.Add(actVeh);
            }
            context?.Dispose();

            return list;
        }
    }
}