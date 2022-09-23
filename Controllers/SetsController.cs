using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using mpvv2.DbModels;
using mpvv2.Models;
using mpvv2.Models.DBModels;

namespace mpvv2.Controllers
{
    public class SetsController : Controller
    {
        
        public class RemoveSetData
        {
            public string id { get; set; }
            public string toDate { get; set; }
            public bool toHistory { get; set; }
        }
        
        public class AddSetData
        {
            public string fromDate { get; set; }
            public string vehRegNum1 { get; set; }
            public string vehRegNum2 { get; set; }
            public string vehRegNum3 { get; set; }
        }
        
        public ActionResult Index()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [Route("sets/getsets")]
        [Produces("application/json")]
        public ActionResult GetSets()
        {
            try
            {
                List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                using (var context = new mpvContext())
                {
                    var sets = context.VehSets.OrderBy(o=>o.IdVeh1Navigation.LongRegNum).ToList();
                    foreach (var set in sets)
                    {
                        Dictionary<string, object> actSet = new Dictionary<string, object>();
                        List<object> vehs = new List<object>();
                        
                        {
                            Dictionary<string, object> actVeh = new Dictionary<string, object>();
                            var veh = context.Vehicles.FirstOrDefault(v => v.Id == set.IdVeh1);
                            actVeh.Add("id",set.IdVeh1);
                            if (veh != null)
                            {
                                if(veh.RegNum.IsNullOrWhiteSpace())
                                    actVeh.Add("regNum",veh.LongRegNum);
                                else
                                    actVeh.Add("regNum",veh.RegNum);
                                vehs.Add(actVeh);
                            }
                        }
                        {
                            Dictionary<string, object> actVeh = new Dictionary<string, object>();
                            var veh = context.Vehicles.FirstOrDefault(v => v.Id == set.IdVeh2);
                            actVeh.Add("id",set.IdVeh2);
                            if (veh != null)
                            {
                                if(veh.RegNum.IsNullOrWhiteSpace())
                                    actVeh.Add("regNum",veh.LongRegNum);
                                else
                                    actVeh.Add("regNum",veh.RegNum);
                                vehs.Add(actVeh);
                            }
                        }
                        {
                            Dictionary<string, object> actVeh = new Dictionary<string, object>();
                            var veh = context.Vehicles.FirstOrDefault(v => v.Id == set.IdVeh3);
                            actVeh.Add("id",set.IdVeh3);
                            if (veh != null)
                            {
                                if(veh.RegNum.IsNullOrWhiteSpace())
                                    actVeh.Add("regNum",veh.LongRegNum);
                                else
                                    actVeh.Add("regNum",veh.RegNum);
                                vehs.Add(actVeh);
                            }
                        }
                        actSet.Add("id",set.Id);
                        actSet.Add("dateFrom",((DateTimeOffset)set.DateFrom).ToUnixTimeSeconds());
                        actSet.Add("vehs",vehs);
                        data.Add(actSet);
                    }
                }

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
                Console.WriteLine("Cannot add set, an error occured: " + e.Message);
            }
            return Json(new {success = false});
        }
        
        [Route("sets/removeset")]
        [Produces("application/json")]
        public ActionResult RemoveSet([FromBody]RemoveSetData formData)
        {
            int setInt = Helper.getInt(formData.id);
            if (setInt != -1)
            {
                try
                {
                    using (var context = new mpvContext())
                    {
                        var set = context.VehSets.FirstOrDefault(s => s.Id == setInt);

                        if (set != null)
                        {
                            DateTime dateTo = DateTime.Now;
                            try
                            {
                                dateTo = DateTime.Parse(formData.toDate);
                            }
                            catch (Exception ex)
                            {
                                return Json(new {success = false, data = "Invalid date provided"});
                            }
                            
                            if (formData.toHistory)
                            {
                                VehSetHistory setHistory = new VehSetHistory()
                                {
                                    DateFrom = set.DateFrom,
                                    DateTo = dateTo,
                                    IdVeh1 = set.IdVeh1,
                                    IdVeh2 = set.IdVeh2,
                                    IdVeh3 = set.IdVeh3
                                };
                                context.VehSetHistories.Add(setHistory);
                            }

                            VehicleManager.UpdateDepartsSet(set.IdVeh1, set.IdVeh2, set.IdVeh3, dateTo, DateTime.Now, context);
                            
                            context.VehSets.Remove(set);
                            context.SaveChanges();
                            return Json(new {success = true});
                        }
                        return Json(new {success = false, data = "This set does not exist"});
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot remove set, an error occured: " + e.Message);
                    return Json(new {success = false, data = "Cannot remove set! Please try again later."});
                }
            }

            return Json(new {success = false, data = "Invalid set id provided!"});
        }

        [Route("sets/addset")]
        [Produces("application/json")]
        public ActionResult AddSet([FromBody]AddSetData formData)
        {
            try
            {
                using (var context = new mpvContext())
                {
                    VehSet set = new VehSet()
                    {
                        DateFrom = DateTime.Parse(formData.fromDate)
                    };

                    if (!formData.vehRegNum1.IsNullOrWhiteSpace())
                    {
                        var veh = context.Vehicles.FirstOrDefault(s => s.RegNum == formData.vehRegNum1 || s.LongRegNum == formData.vehRegNum1);
                        if(veh == null)
                            return Json(new {success = false, data = "First vehicle in set does not exist"});
                        set.IdVeh1 = veh.Id;
                    }
                    if (!formData.vehRegNum2.IsNullOrWhiteSpace())
                    {
                        var veh = context.Vehicles.FirstOrDefault(s => s.RegNum == formData.vehRegNum2 || s.LongRegNum == formData.vehRegNum2);
                        if(veh == null)
                            return Json(new {success = false, data = "Second vehicle in set does not exist"});
                        set.IdVeh2 = veh.Id;
                    }
                    if (!formData.vehRegNum3.IsNullOrWhiteSpace())
                    {
                        var veh = context.Vehicles.FirstOrDefault(s => s.RegNum == formData.vehRegNum3 || s.LongRegNum == formData.vehRegNum3);
                        if(veh != null)
                            set.IdVeh3 = veh.Id;
                    }
                    
                    context.VehSets.Add(set);
                    VehicleManager.UpdateDepartsSet(set.IdVeh1, set.IdVeh2, set.IdVeh3, DateTime.Parse(formData.fromDate), DateTime.Now, context);
                    
                    context.SaveChanges();
                    return Json(new {success = true});
                }
            }
            catch (Exception e)
            {
                return Json(new {success = false, data = "Cannot add set! Please try again later."});
            }
        }
    }
}