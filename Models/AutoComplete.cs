using System;
using System.Collections.Generic;
using System.Linq;
using mpvv2.Models.DBModels;

namespace mpvv2.Models
{
    public class AutoComplete
    {
        public AutoComplete()
        {
            
        }

        public string ListToJsonString(List<Dictionary<string,dynamic>> list)
        {
            if (list == null)
                return "[]";
            string str = "[";
            bool first = true;
            foreach (var item in list)
            {
                if (!first)
                    str += ",";
                first = false;
                str += "{";
                bool fst = true;
                foreach (var pair in item)
                {
                    if (!fst)
                        str += ",";
                    fst = false;
                    str += "\"" + pair.Key + "\": \"" + pair.Value + "\"";
                }
                str += "}";
            }

            str += "]";
            return str;
        }

        public List<Dictionary<string,dynamic>> GetAutocompleteVehicle(string str)
        {
            if (!ModelsManager.isValidStringForDatabase(str))
                return null;
            using (var db = new Database())
            {
                string sql =
                    "SELECT * FROM vehicle v LEFT JOIN veh_up_type u ON (u.id = v.id_vut) INNER JOIN veh_type t ON(t.id=u.id_vet) INNER JOIN manufacturer m ON (m.id = t.id_man) LEFT JOIN depot e ON (e.id = v.id_dep) INNER JOIN carrier c ON (e.id_car = c.id)" +
                    " WHERE v.reg_num LIKE \"%" + str + "%\" OR v.long_reg_num LIKE \"%" + str +
                    "%\" GROUP BY v.id ORDER BY v.long_reg_num LIMIT 7";
                Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
                var list = db.Select(sql, parameters);

                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].ContainsKey("last_seen"))
                    {
                        var lastSeenType =
                            ModelsManager.getDateDifferenceType(DateTime.Parse(list[i]["last_seen"].ToString()),
                                DateTime.Now);
                        var lastSeenString =
                            ModelsManager.getDateDifferenceString(DateTime.Parse(list[i]["last_seen"].ToString()),
                                DateTime.Now);
                        list[i].Add("last_seen_type", lastSeenType);
                        list[i].Add("last_seen_string", lastSeenString);
                    }
                }

                return list;
            }
        }

        public List<Dictionary<string,dynamic>> GetAutocompleteLine(string str)
        {
            if (!ModelsManager.isValidStringForDatabase(str))
                return null;
            using (var db = new Database())
            {
                string sql = "SELECT line FROM depart WHERE line LIKE \"%" + str +
                             "%\" GROUP BY line ORDER BY line LIMIT 7";
                Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
                var list = db.Select(sql, parameters);

                return list;
            }
        }
        
        public List<Dictionary<string,dynamic>> GetAutocompleteCarrier(string str)
        {
            if (!ModelsManager.isValidStringForDatabase(str))
                return null;
            using (var db = new Database())
            {
                string sql = "SELECT name FROM carrier WHERE name LIKE \"%" + str + "%\" ORDER BY name LIMIT 7";
                Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
                var list = db.Select(sql, parameters);

                return list;
            }
        }
        
        public List<Dictionary<string,dynamic>> GetAutocompleteType(string str)
        {
            if (!ModelsManager.isValidStringForDatabase(str))
                return null;
            using (var db = new Database())
            {
                string sql = "SELECT name FROM veh_up_type WHERE name LIKE \"%" + str + "%\" ORDER BY name LIMIT 7";
                Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
                var list = db.Select(sql, parameters);

                return list;
            }
        }
        
        public List<Dictionary<string,dynamic>> GetAutocompleteManufacturer(string str)
        {
            if (!ModelsManager.isValidStringForDatabase(str))
                return null;
            using (var db = new Database())
            {
                string sql = "SELECT name FROM manufacturer WHERE name LIKE \"%"+str+"%\" ORDER BY name LIMIT 7";
                Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
                var list = db.Select(sql, parameters);
            
                return list;
            }
        }
    }
}