
using Microsoft.AspNetCore.Mvc;
using mpvv2.Models;

namespace mpvv2.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult Index(bool data)
        {
            if (data)
                return Json(new { showNav=true, title="Search" });
                
            ViewBag.Message = "Your contact page.";
            return View();
        }
        
        [HttpPost] 
        public ActionResult LoadDeparts(string orderd, string limit, string line, string veh_id, string day, string fromDate, string toDate, string carrier, string veh_type, string veh_manuf, string route, string descd)
        {
            if (orderd.IsNullOrWhiteSpace() || limit.IsNullOrWhiteSpace())
                return Content("Bad arguments.");
            bool desc = !descd.IsNullOrWhiteSpace();

            var dl = new DepartsLoader();
            var list = dl.LoadDeparts(line,veh_id,day,fromDate,toDate,orderd,desc,Helper.getInt(limit),carrier,veh_type,veh_manuf,route);
            string jsonString = dl.GetJsonFromDepartsList(list);
            return Content(jsonString);
        }
        
        [HttpPost] 
        public ActionResult LoadVehicles(string order, string limit, string line, string veh_id, string day, string fromDate, string toDate, string carrier, string veh_type, string veh_manuf, string route, string desc)
        {
            if (order.IsNullOrWhiteSpace() || limit.IsNullOrWhiteSpace())
                return Content("Bad arguments.");

            bool des = !desc.IsNullOrWhiteSpace();

            var dl = new VehiclesLoader();
            var list = dl.LoadVehicles(line,veh_id,day,fromDate,toDate,order,des,Helper.getInt(limit),carrier,veh_type,veh_manuf,route);
            string jsonString = dl.GetJsonFromVehiclesList(list);
            return Content(jsonString);
        }
    }
}