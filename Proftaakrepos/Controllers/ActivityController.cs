using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proftaakrepos.Authorize;
using CookieManager;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Authentication;
using Logic.API;
using DAL;
using System.Threading;
using System.Globalization;

namespace Proftaakrepos.Controllers
{
    public class ActivityController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TempData["Cookie"] = HttpContext.Session.GetString("UserInfo");
            string language = HttpContext.Session.GetString("Culture");
            if (!string.IsNullOrEmpty(language))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            }
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