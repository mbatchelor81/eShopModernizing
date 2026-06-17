using System.Net;
using System.Web.Mvc;

namespace eShopLegacyMVC.Controllers
{
    public class HealthController : Controller
    {
        [HttpGet]
        [Route("health")]
        public ActionResult Index()
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
