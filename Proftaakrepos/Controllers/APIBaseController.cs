using Logic.HoursWorked;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Proftaakrepos.Controllers
{
    public class APIBaseController : ControllerBase
    {
        [Route("api/incidenten")]
        public ActionResult<string[]> GetIncidentenFromUser()
        {
            return new TimeSheetManager().GetUsersIncidentIDs(HttpContext.Session.GetInt32("UserInfo.ID").ToString());
        }
    }
}