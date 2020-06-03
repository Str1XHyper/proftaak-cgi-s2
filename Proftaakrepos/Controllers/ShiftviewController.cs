using DAL;
using CookieManager;
using FullCalendar;
using Logic.Ruilverzoeken;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Authentication;
using Proftaakrepos.Authorize;
using System;
using System.Globalization;
using System.Threading;

namespace Proftaakrepos.Controllers
{
    public class ShiftviewController : Controller
    {
        private readonly ICookieManager _cookieManager;
        private readonly IDataManager dataManager;
        public ShiftviewController(ICookieManager cookiemanager, IDataManager dataManager)
        {
            _cookieManager = cookiemanager;
            this.dataManager = dataManager;
        }
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
        [UserAccess("", "Reactie op verzoek")]
        public IActionResult ShiftviewEmail()
        {
            return View();
        }
        [UserAccess("","Inkomend")]
        public IActionResult Incoming()
        {
            int UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserInfo.ID"));
            ViewData["CurrentID"] = UserID;
            ViewData["tradeRequests"] = dataManager.GetRequests();
            ViewData["users"] = dataManager.GetUsers();
            return View();
        }
        [UserAccess("", "Uitgaand")]
        public IActionResult CreateRequest(string status)
        {
            if (status != null) ViewData["Status"] = status;
            else ViewData["Status"] = string.Empty;

            int UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserInfo.ID"));
            ViewData["CurrentID"] = UserID;
            ViewData["dienstInfo"] = dataManager.GetDiensten(UserID);
            ViewData["tradeRequests"] = dataManager.GetRequests();
            ViewData["users"] = dataManager.GetUsers();
            return View();
        }
        [UserAccess("", "Uitgaand")]
        [HttpPost]
        public IActionResult CreateRequest(string EventID, string UserID)
        {
            if (UserID.EndsWith(",")) UserID = UserID.Substring(0, UserID.Length - 1);
            if (EventID == "0")
            {
                return RedirectToAction("CreateRequest", new { status = "Geen dienst geselecteerd" });
            }
            dataManager.AddRequest(EventID, UserID);
            return RedirectToAction("CreateRequest");
        }

        // Incomming trade requests
        [UserAccess("", "Inkomend")]
        public IActionResult AcceptRequest(string UserID, int TradeID, int EventID)
        {
            dataManager.AcceptTradeRequest(UserID, TradeID, EventID);
            return Redirect("Incoming");
        }
        [UserAccess("", "Inkomend")]
        public IActionResult Block(string UserID, int TradeID, string DisabledIds)
        {
            dataManager.BlockRequest(UserID, TradeID, DisabledIds);
            return Redirect("Incoming");
        }

        // Outgoing trade requests
        [UserAccess("", "Uitgaand")]
        public IActionResult Cancel(string EventID)
        {
            dataManager.CancelTradeRequest(EventID);
            return RedirectToAction("CreateRequest", new { status = "U heeft uw aanvraag geannuleerd" });
        }
    }
}