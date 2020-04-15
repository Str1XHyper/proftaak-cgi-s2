using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proftaakrepos.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using ClassLibrary.Classes;

namespace Proftaakrepos.Controllers
{
    public class ShiftviewController : Controller
    {
        public IActionResult Incoming()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return View();
        }

        public IActionResult SorryXander()
        {
            return View();
        }

        public IActionResult CreateRequest()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return View();
        }

        [HttpPost]
        public IActionResult CreateRequest(string EventID, string UserID)
        {
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
            string[] query = new string[] { $"Update TradeRequest Set Status = 1 Where TradeId = {TradeID}", $"Update TradeRequest Set UserIdAcceptor = {UserID} Where TradeId = {TradeID} " };
            SQLConnection.ExecuteNonSearchQueryArray(query);
            //SQLConnection.ExecuteNonSearchQuery($"Update TradeRequest Set UserIdAcceptor = {UserID} Where TradeId = {TradeID}");
            SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set UserId = {UserID}, IsPending = 0 Where EventId = {EventID}");
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
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