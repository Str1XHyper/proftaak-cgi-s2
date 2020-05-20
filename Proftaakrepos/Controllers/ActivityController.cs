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

namespace Proftaakrepos.Controllers
{
    public class ActivityController : Controller
    {
        private readonly ICookieManager _cookieManager;
        public ActivityController(ICookieManager cookiemanager)
        {
            _cookieManager = cookiemanager;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TempData["CookieMonster"] = _cookieManager.Get<CookieModel>("BIER.User");
        }
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