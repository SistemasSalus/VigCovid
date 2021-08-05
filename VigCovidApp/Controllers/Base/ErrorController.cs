using System.Web.Mvc;

namespace VigCovidApp.Controllers.Base
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }
    }
}