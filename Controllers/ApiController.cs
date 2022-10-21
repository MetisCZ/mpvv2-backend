using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using mpvv2.DbModels;
using mpvv2.Models;
using mpvv2.Models.DBModels;

namespace mpvv2.Controllers
{
    public class ApiController : Controller
    {
        public ActionResult Index()
        {
            var context = new mpvContext();
            var res = (from d in context.Depart
                join v in context.Vehicles on d.IdVeh equals v.Id
                where v.LongRegNum == "410284"
                select new { v, d }).ToList();

            //VehicleManager.UpdateDepartsSet("554", null, null, DateTime.Parse("05/09/2022"), DateTime.Today, context);
            //VehicleManager.UpdateVehicle("555", context);

            context.SaveChanges();
            context.Dispose();
            return Content("Done");
        }

        [Route("api/updatevehicledata/{vehId}")]
        [Produces("application/json")]
        public ActionResult UpdateVehicleData(string vehId)
        {
            using (var context = new mpvContext())
            {
                Vehicle veh = context.Vehicles.FirstOrDefault(v => v.Id == vehId);
                if(veh == null)
                    return Content("{\"success\":\"false\",\"data\":\"Invalid vehicle id\"}");
                Depart dep = context.Depart.Where(d => d.IdVeh == vehId || d.IdVeh2 == vehId || d.IdVeh3 == vehId).OrderByDescending(d=>d.ActDate).FirstOrDefault();
                if (dep != null)
                    veh.LastSeen = dep.ActDate;
            }
            return Content("{\"success\":\"true\",\"data\":\"\"}");
        }

        [Route("api/autocomplete/{type}/{search}")]
        public ActionResult AutoComplete(string type, string search)
        {
            string str = "{}";
            var ac = new AutoComplete();
            switch (type.ToLower())
            {
                case "vehicle": str = ac.ListToJsonString(ac.GetAutocompleteVehicle(search)); break;
                case "line": str = ac.ListToJsonString(ac.GetAutocompleteLine(search)); break;
                case "carrier": str = ac.ListToJsonString(ac.GetAutocompleteCarrier(search)); break;
                case "manufacturer": str = ac.ListToJsonString(ac.GetAutocompleteManufacturer(search)); break;
                case "vehtype": str = ac.ListToJsonString(ac.GetAutocompleteType(search)); break;
            }
            return Content(str);
        }

        [Route("api/setwasin/{idDep}/{idVeh}")]
        public ActionResult SetWasIn(string idDep, string idVeh)
        {
            if (idDep.IsNullOrWhiteSpace() || idVeh.IsNullOrWhiteSpace())
            {
                return Content("{\"success\": false,\"message\": \"Invalid arguments\"}");
            }

            using (var context = new mpvContext())
            {
                long idD = long.Parse(idDep);
                Depart dep =
                    context.Depart.FirstOrDefault(d => d.IdVeh.Equals(idVeh) && d.Id == idD);
                Vehicle veh = context.Vehicles.FirstOrDefault(v => v.Id.Equals(idVeh));
                if(dep == null || veh == null)
                    return Content("{\"success\": false,\"message\": \"Invalid data provided. Please try again later\"}");
                if(dep.WasIn)
                    return Content("{\"success\": false,\"message\": \"You've already been on this route\"}");
                dep.WasIn = true;
                veh.WasInCount++;
                veh.WasInLast = DateTime.Now;
                if (context.SaveChanges() == 0)
                {
                    return Content("{\"success\": false,\"message\": \"Something went wrong. Please try again later\"}");
                }
                return Content("{\"success\": true,\"message\": \"Updated\"}");
            }

            return Content("");
        }

        [Route("api/getvehicledata/{vehicle}")]
        public ActionResult GetVehicleData(string vehicle)
        {
            if (vehicle.IsNullOrWhiteSpace())
            {
                return Content("{\"success\": false,\"message\": \"Invalid arguments\"}");
            }
            using (var db = new Database())
            {
                string sql =
                    "SELECT d.line,d.route, v.reg_num, d.act_date, d.id AS id_dep, v.id AS id_veh FROM depart d INNER JOIN vehicle v ON (d.id_veh = v.id) WHERE v.id = @vehicle ORDER BY d.act_date DESC LIMIT 1;";
                Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
                parameters.Add("@vehicle", vehicle);
                var res = db.Select(sql, parameters);
                if(res.Count() != 1)
                    return Content("{\"success\": false,\"message\": \"No data found\"}");
                var date = ModelsManager.getFormattedDateTime(DateTime.Parse(res[0]["act_date"].ToString()));
                return Content("{\"success\":true,\"data\": { \"vehicle\": \""+res[0]["reg_num"]+"\", \"line\": \""+res[0]["line"]+"\", \"route\": \""+res[0]["route"]+"\", \"date\": \""+date+"\", \"id_dep\": \""+res[0]["id_dep"]+"\", \"id_veh\": \""+res[0]["id_veh"]+"\"}}");
            }

            return Content("");
        }

        [Route("api/getlinedata/{line}/{route}/{vehicle}/{datetime}")]
        public ActionResult GetLineData(string line, string route, string vehicle, string datetime)
        {
            if(line.IsNullOrWhiteSpace() || route.IsNullOrWhiteSpace() || vehicle.IsNullOrWhiteSpace() || datetime.IsNullOrWhiteSpace())
            {
                return Content("{\"success\": false,\"message\": \"Invalid arguments\"}");
            }
            using (var db = new Database())
            {
                string sql =
                    "SELECT d.line,d.route, v.reg_num, d.act_date, d.id AS id_dep, v.id AS id_veh FROM depart d INNER JOIN vehicle v ON (d.id_veh = v.id) WHERE d.line = @line AND d.route = @route AND d.date = @date AND v.id = @vehicle LIMIT 1;";
                Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
                parameters.Add("@vehicle", vehicle);
                parameters.Add("@line", line);
                parameters.Add("@route", route);
                parameters.Add("@date", datetime);
                var res = db.Select(sql, parameters);
                if(res.Count() != 1)
                    return Content("{\"success\": false,\"message\": \"No data found\"}");
                var date = ModelsManager.getFormattedDateTime(DateTime.Parse(res[0]["act_date"].ToString()));
                return Content("{\"success\":true,\"data\": { \"vehicle\": \""+res[0]["reg_num"]+"\", \"line\": \""+res[0]["line"]+"\", \"route\": \""+res[0]["route"]+"\", \"date\": \""+date+"\", \"id_dep\": \""+res[0]["id_dep"]+"\", \"id_veh\": \""+res[0]["id_veh"]+"\"}}");
            }
            
            return Content("");
        }

        public ActionResult UpdateData(string args)
        {
            int type = 1;
            if (!string.IsNullOrWhiteSpace(args))
            {
                switch (args.ToLower())
                {
                    case "pid":
                        type = 2;
                        break;
                }
            }
            string date = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");

            var ra = new RecordAdder();
            ra.GenerateRecord(1);
            return Content("{ \"date\": \""+date+"\",\"message\": \"\"}");
        }
    }
}