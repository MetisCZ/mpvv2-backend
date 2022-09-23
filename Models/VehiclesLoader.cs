using System;
using System.Collections.Generic;
using System.Linq;
using mpvv2.Models.DBModels;

namespace mpvv2.Models
{
    public class VehiclesLoader
    {
        public string GetJsonFromVehiclesList(List<Dictionary<string, object>> list)
        {
            string res = "[";
            int i = 0;
            foreach (Dictionary<string,object> item in list)
            {
                if (i != 0)
                    res += ",";
                i++;
                string manufYear = ModelsManager.getFormattedCustomDate(item["manufac_year"].ToString());
                string ac = ModelsManager.getAirConditionAsStringStatic(Helper.getInt(item["air_condition"].ToString()));
                int lastSeenType = 0;
                string lastSeenDate = "";
                string lastSeen = "bez dat";
                if (item.GetValue("last_seen") != null)
                {
                    lastSeenType = ModelsManager.getDateDifferenceType(DateTime.Parse(item.GetValue("last_seen").ToString()), DateTime.Now);
                    lastSeenDate = ModelsManager.getFormattedDateTimeNoSecs(DateTime.Parse(item.GetValue("last_seen").ToString()));
                    lastSeen = ModelsManager.getDateDifferenceString(DateTime.Parse(item.GetValue("last_seen").ToString()), DateTime.Now);   
                }
                string wasIn = "";
                if(item.GetValue("was_in_last") != null)
                    wasIn = ModelsManager.getFormattedDateTime(DateTime.Parse(item.GetValue("was_in_last").ToString()));
                res += "{\"id\": \"" + item.GetValue("id") + "\", \"count\": \"" + item.GetValue("count") +
                       "\", \"long_reg_num\": \"" + item.GetValue("long_reg_num") + "\", " +
                       "\"reg_num\": \"" + item.GetValue("reg_num") + "\", \"manufactured_date\":\"" + manufYear +
                       "\", \"manufacturer_name\":\"" + item.GetValue("manufacturer_name") + "\", " +
                       "\"vehicle_type\":\"" + item.GetValue("vehicle_type") + "\", \"air_condition\":\"" + ac +
                       "\", \"carrier_name\":\"" + item.GetValue("carrier_name") + "\", " +
                       "\"last_seen_date\":\"" + lastSeenDate + "\", \"last_seen\":\"" + lastSeen +
                       "\", \"was_in_count\":\"" + item.GetValue("was_in_count") + "\", " +
                       "\"was_in_last\":\"" + wasIn + "\", \"last_seen_type\":\"" + lastSeenType + "\" }";
            }

            res += "]";
            return res;
        }

        public List<Dictionary<string, object>> LoadVehicles(string line=null, string vehId=null, string day=null, string fromDate=null, string toDate=null, string order=null, bool isDesc=false, int limit=1000, string carrier=null, string vehType=null, string manuf=null, string route=null)
        {
            // depart        - d
            // carrier       - c
            // stop          - s
            // vehicle       - v
            // vehicle_manuf - m
            // vehicle_type  - t
            
            string desc = "ASC";
            if (order.IsNullOrWhiteSpace() || order == "default")
                order = "id_car, long_reg_num";
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
                "SELECT v.id,COUNT(d.id) AS count, v.long_reg_num, v.reg_num,c.name AS carrier_name,v.manufac_year,m.name AS manufacturer_name, " +
                "u.name AS vehicle_type,v.air_condition,v.last_seen,v.was_in_last,v.was_in_count"+
                " FROM vehicle v LEFT JOIN depart_odis d ON (v.ID = d.id_veh) LEFT JOIN veh_up_type u ON (u.id = v.id_vut)" +
                " INNER JOIN veh_type t ON(t.id=u.id_vet) INNER JOIN manufacturer m ON (m.id = t.id_man)" +
                " LEFT JOIN depot e ON (e.id = v.id_dep) INNER JOIN carrier c ON (e.id_car = c.id)"+
                whereQuery+
                " GROUP BY v.id"+
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

            //Console.WriteLine(res.Count());
            return res;
        }
    }
}