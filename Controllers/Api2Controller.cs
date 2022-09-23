using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using mpvv2.DbModels;
using mpvv2.Models;
using mpvv2.Models.DBModels;

namespace mpvv2.Controllers
{
    public class Api2Controller : Controller
    {
        public class SearchData
        {
            public string part { get; set; }
            public string fromDate { get; set; }
            public string toDate { get; set; }
            public string line { get; set; }
            public string route { get; set; }
            public string vehicle { get; set; }
            public int limit { get; set; }
        }

        public class VehicleData
        {
            public string id { get; set; }
        }

        [Route("api2/search/vehicles")]
        [Produces("application/json")]
        public ActionResult SearchVehicles([FromBody]SearchData postData)
        {
            try
            {
                var vehiclesModel = new VehiclesModel();
                List<Dictionary<string, object>> data = vehiclesModel.GetVehiclesList(postData);
                Dictionary<string, object> list = new Dictionary<string, object>();
                list.Add("success",true);
                list.Add("data",data);
                return Json(list, new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return Json(new { success = false });
            }
        }
        
        [Route("api2/search/departs")]
        [Produces("application/json")]
        public ActionResult SearchDeparts([FromBody]SearchData postData)
        {
            try
            {
                var vehiclesModel = new VehiclesModel();
                List<Dictionary<string, object>> data = vehiclesModel.GetDepartsList(postData);
                Dictionary<string, object> list = new Dictionary<string, object>();
                list.Add("success",true);
                list.Add("data",data);
                return Json(list, new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return Json(new { success = false });
            }
        }
        
        [Route("api2/nowActive")]
        [Produces("application/json")]
        public ActionResult GetNowActive()
        {
            try
            {
                var vehiclesModel = new VehiclesModel();
                List<Dictionary<string, object>> data = vehiclesModel.GetNowActive(null);
                Dictionary<string, object> list = new Dictionary<string, object>();
                list.Add("success",true);
                list.Add("data",data);
                return Json(list, new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
        }

        [Route("api2/vehicle")]
        [Produces("application/json")]
        public ActionResult GetVehicleData([FromBody]VehicleData postData)
        {
            try
            {
                var vehiclesModel = new VehiclesModel();
                Dictionary<string, object> data = vehiclesModel.GetVehicleData(postData);
                Dictionary<string, object> list = new Dictionary<string, object>();
                list.Add("success",true);
                list.Add("data",data);
                return Json(list, new JsonSerializerOptions()
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    });
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return Json(new { success = false });
            }
        }
        
        [Route("api2/refreshVehicle")]
        [Produces("application/json")]
        public ActionResult RefreshVehicleData([FromBody]VehicleData postData)
        {
            try
            {
                if (postData != null && !postData.id.IsNullOrWhiteSpace())
                {
                    var vehiclesModel = new VehiclesModel();
                    using (var context = new mpvContext())
                    {
                        if (VehicleManager.UpdateVehicle(postData.id,context))
                        {
                            Dictionary<string, object> list = new Dictionary<string, object>();
                            list.Add("success",true);
                        
                            context.SaveChanges();
                        
                            return Json(list, new JsonSerializerOptions()
                            {
                                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                            });
                        }
                    }
                }
                return Json(new { success = false });
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return Json(new { success = false });
            }
        }
    }
}