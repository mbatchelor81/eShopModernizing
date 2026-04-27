using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace eShopLegacyMVC.Controllers
{
    [RoutePrefix("health")]
    public class HealthController : ApiController
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { status = "Healthy" });
        }

        [HttpGet]
        [Route("ready")]
        [AllowAnonymous]
        public HttpResponseMessage Ready()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { status = "Ready" });
        }
    }
}
