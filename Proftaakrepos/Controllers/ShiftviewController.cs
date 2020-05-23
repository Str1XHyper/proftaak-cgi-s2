using ClassLibrary.Classes;
using CookieManager;
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
        public ShiftviewController(ICookieManager cookiemanager)
        {
            _cookieManager = cookiemanager;
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
            return View();
        }
        [UserAccess("", "Uitgaand")]
        public IActionResult CreateRequest(string status)
        {
            if (status != null) ViewData["Status"] = status;
            else ViewData["Status"] = string.Empty;
            return View();
        }
        [UserAccess("", "Uitgaand")]
        [HttpPost]
        public IActionResult CreateRequest(string EventID, string UserID)
        {
            if( EventID == "0")
            {
                return RedirectToAction("CreateRequest", new { status = "Geen dienst geselecteerd"});
            }

            string[] roosterData = SQLConnection.ExecuteSearchQuery($"Select * from Rooster where EventId = {EventID}").ToArray();

            DateTime start = DateTime.Parse(roosterData[4]);
            DateTime end = DateTime.Parse(roosterData[5]);
            DateTime startnew = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second);
            DateTime endnew = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second);

            SQLConnection.ExecuteNonSearchQuery($"Insert Into `TradeRequest`(`UserIdIssuer`, `Status`, `Start`, `End`, `UserIdAcceptor`, `DisabledIDs`,`EventID`) values({UserID}, 0, '{startnew.ToString("yyyy/MM/dd HH:mm")}', '{endnew.ToString("yyyy/MM/dd HH:mm")}', -1, 0, '{EventID}')");
            SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set IsPending = 1 where EventId = {EventID}");

            return RedirectToAction("CreateRequest");
        }
        [UserAccess("", "Inkomend")]
        public IActionResult HandleRequest(string UserID, int TradeID, int EventID)
        {
            string nameAcceptor;
            string[] query = new string[] { $"Update TradeRequest Set Status = 1 Where TradeId = {TradeID}", $"Update TradeRequest Set UserIdAcceptor = {UserID} Where TradeId = {TradeID} " };
            SQLConnection.ExecuteNonSearchQueryArray(query);
            //SQLConnection.ExecuteNonSearchQuery($"Update TradeRequest Set UserIdAcceptor = {UserID} Where TradeId = {TradeID}");
            SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set UserId = {UserID}, IsPending = 0 Where EventId = {EventID}");
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            string[] userData = SQLConnection.ExecuteSearchQuery($"SELECT * FROM Werknemers Where UserId='{UserID}'").ToArray();
            string[] eventData = SQLConnection.ExecuteSearchQuery($"SELECT * FROM Rooster Where EventId='{EventID}'").ToArray();
            if(userData[2] == "")
            {
                nameAcceptor = userData[1] + " " + userData[3];
            }
            else
            {
                nameAcceptor = userData[1] + " " + userData[2] + " " + userData[3];
            }
            return Redirect("Incoming");
        }
        [UserAccess("", "Inkomend")]
        public IActionResult Block(string UserID, int TradeID, string DisabledIds)
        {
            SQLConnection.ExecuteNonSearchQuery($"Update TradeRequest Set DisabledIds = '{DisabledIds},{UserID}' Where TradeId = {TradeID}");
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return Redirect("Incoming");
        }
        [UserAccess("", "Uitgaand")]
        public IActionResult Cancel(string EventID)
        {
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM `TradeRequest` WHERE `EventID` = {EventID}; UPDATE `Rooster` SET `IsPending` = 0 WHERE `EventId` = {EventID}");
            return RedirectToAction("CreateRequest", new { status = "U heeft uw aanvraag geannuleerd" });
        }
    }
}