using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proftaakrepos.Authorize;
using ClassLibrary.Classes;
namespace Proftaakrepos.Controllers
{
    public class ActivityController : Controller
    {
        [UserAccess("LoggedIn", "")]
        public IActionResult Index()
        {
            string authcode = HttpContext.Session.GetString("UserInfo");
            List<string[]> responseRecords = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT * FROM `Logins` WHERE `AuthCode` = '{authcode}'");
            ViewData["records"] = responseRecords;
            AddLoginLog addLoginLog = new AddLoginLog();
            ViewData["ip"] = addLoginLog.CallUrl("https://api.ipify.org/");
            return View();
        }
    }
}