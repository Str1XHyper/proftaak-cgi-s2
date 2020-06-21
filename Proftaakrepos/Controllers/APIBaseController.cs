using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Proftaakrepos.Controllers
{
    public class APIBaseController : ControllerBase
    {
        [Route("api/incidenten")]
        public ActionResult<string[]> GetIncidentenFromUser()
        {
            string user = HttpContext.Session.GetInt32("UserInfo.ID").ToString();
            return new string[0];
        }
    }
}