using mpvv2.Models;
using Microsoft.AspNetCore.Mvc;

namespace mpvv2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(bool data, bool mini)
        {
            if (data)
                return Json(new { showNav=true, title="Home" });
            
            //var ra = new RecordAdder();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}