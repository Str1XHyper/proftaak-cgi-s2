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
    public class HomeController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return Redirect("Agenda");
        }

        public IActionResult NoAccessIndex()
        {
            ModelState.AddModelError("", "U heeft niet de rechten om deze pagina te bezoeken.");
            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Employees()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
        
        public IActionResult HandleRequest(string UserID, int TradeID)
        {

            MySqlConnection cnn;
            string connetionString = "server=185.182.57.161;database=tijnvcd415_Proftaak;uid=tijnvcd415_Proftaak;pwd=Proftaak;";
            cnn = new MySqlConnection(connetionString);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = $"Update TradeRequest Set Status = 1 Where TradeId = {TradeID} ";
            try
            {
                cnn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                //"Can not open connection ! " + ex.Message.ToString()
                return View("ShiftView");
            }

            cmd.CommandText = $"Update TradeRequest Set UserIdAcceptor = {UserID} Where TradeId = {TradeID}";
            try
            {
                cnn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                //"Can not open connection ! " + ex.Message.ToString()
                return View("ShiftView");
            }
            return RedirectToAction("ShiftView", "Home");
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
            } else
            {
                startDates = roosterData[4].Split(" ")[0].Split("-");
                startTime = roosterData[4].Split(" ")[1];
                endDates = roosterData[5].Split(" ")[0].Split("-");
                endTime = roosterData[5].Split(" ")[1];
            }

            SQLConnection.ExecuteNonSearchQuery($"Insert Into `TradeRequest`(`UserIdIssuer`, `Status`, `Start`, `End`, `UserIdAcceptor`, `DisabledIDs`) values({UserID}, 0, '{startDates[2]}-{startDates[1]}-{startDates[0]} {startTime}', '{startDates[2]}-{startDates[1]}-{startDates[0]} {startTime}', -1, 0)");
            SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set IsPending = 1 where EventId = {EventID}");
            
            return RedirectToAction("ShiftView", "Home");
        }
    }
}
