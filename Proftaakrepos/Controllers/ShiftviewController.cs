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

        public IActionResult CreateRequest()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return View();
        }


        public IActionResult HandleRequest(string UserID, int TradeID)
        {
            string[] query = new string[] { $"Update TradeRequest Set Status = 1 Where TradeId = {TradeID}", $"Update TradeRequest Set UserIdAcceptor = {UserID} Where TradeId = {TradeID} " };
            SQLConnection.ExecuteNonSearchQueryArray(query);
            //SQLConnection.ExecuteNonSearchQuery($"Update TradeRequest Set UserIdAcceptor = {UserID} Where TradeId = {TradeID}");
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