using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using ClassLibrary.Classes;

namespace Proftaakrepos.Controllers
{
    public class ShiftviewController : Controller
    {
        public IActionResult ShiftviewEmail()
        {
            string _authCode = HttpContext.Session.GetString("UserInfo");
            if (CheckIfAllowed.IsAllowed(_authCode, "ShiftViewEmail")) return View();
            else return RedirectToAction("NoAccessIndex", "Home");
        }
        public IActionResult Incoming()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            string _authCode = HttpContext.Session.GetString("UserInfo");
            if (CheckIfAllowed.IsAllowed(_authCode, "Incoming")) return View();
            else return RedirectToAction("NoAccessIndex", "Home");
        }

        public IActionResult CreateRequest(string status)
        {
            if (status != null) ViewData["Status"] = status;
            else ViewData["Status"] = string.Empty;
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            string _authCode = HttpContext.Session.GetString("UserInfo");
            if (CheckIfAllowed.IsAllowed(_authCode, "CreateRequest")) return View();
            else return RedirectToAction("NoAccessIndex", "Home");
        }

        [HttpPost]
        public IActionResult CreateRequest(string EventID, string UserID)
        {
            if( EventID == "0")
            {
                return RedirectToAction("CreateRequest", new { status = "Geen dienst geselecteerd"});
            }

            string[] roosterData = SQLConnection.ExecuteSearchQuery($"Select * from Rooster where EventId = {EventID}").ToArray();

            string[] startDates = new string[3];
            string startTime;
            string[] endDates = new string[3];
            string endTime;

            if (roosterData[4].Split(" ")[0].Contains("/"))
            {
                startDates = roosterData[4].Split(" ")[0].Split("/");
                startTime = roosterData[4].Split(" ")[1];
                endDates = roosterData[5].Split(" ")[0].Split("/");
                endTime = roosterData[5].Split(" ")[1];
            }
            else
            {
                startDates = roosterData[4].Split(" ")[0].Split("-");
                startTime = roosterData[4].Split(" ")[1];
                endDates = roosterData[5].Split(" ")[0].Split("-");
                endTime = roosterData[5].Split(" ")[1];
            }

            SQLConnection.ExecuteNonSearchQuery($"Insert Into `TradeRequest`(`UserIdIssuer`, `Status`, `Start`, `End`, `UserIdAcceptor`, `DisabledIDs`,`EventID`) values({UserID}, 0, '{startDates[2]}-{startDates[1]}-{startDates[0]} {startTime}', '{startDates[2]}-{startDates[1]}-{startDates[0]} {startTime}', -1, 0, '{EventID}')");
            SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set IsPending = 1 where EventId = {EventID}");

            return RedirectToAction("CreateRequest");
        }

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
            #region htmlContent
            string htmlMessage = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
            htmlMessage += "<HTML><HEAD><META http-equiv=Content-Type content=\"text/html; charset=iso-8859-1\">";
            htmlMessage += "</HEAD><BODY><DIV><FONT face=Arial color=#ff0000 size=2>this is some HTML text";
            htmlMessage += "</FONT></DIV></BODY></HTML>";
            #endregion
            string subject = "Ruilverzoek geaccepteerd";
            string plainMessage = $"Test message plz plz plz work {nameAcceptor} accepted your request";
            SendMail.Execute(subject, "alex.peek@hotmail.com", htmlMessage, plainMessage);
            return Redirect("Incoming");
        }

        public IActionResult Block(string UserID, int TradeID, string DisabledIds)
        {
            SQLConnection.ExecuteNonSearchQuery($"Update TradeRequest Set DisabledIds = '{DisabledIds},{UserID}'Where TradeId = {TradeID}");
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return Redirect("Incoming");
        }
    }
}