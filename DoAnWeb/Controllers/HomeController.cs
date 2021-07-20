using System.Web.Mvc;

namespace DoAnWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Tutorial()
        {
            return View();
        }
    }
}