using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proftaakrepos.Authorize;
using CookieManager;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Authentication;
using Logic.API;

namespace Proftaakrepos.Controllers
{
    public class ActivityController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TempData["Cookie"] = HttpContext.Session.GetString("UserInfo");
        }
        [UserAccess("LoggedIn", "")]
        public IActionResult Index()
        {
            string authcode = HttpContext.Session.GetString("UserInfo");
            List<string[]> responseRecords = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT * FROM `Logins` WHERE `AuthCode` = '{authcode}'");
            ViewData["records"] = responseRecords;
            APICalls calls = new APICalls();
            ViewData["ip"] = calls.APICall("https://api.ipify.org/");
            return View();
        }
    }
}