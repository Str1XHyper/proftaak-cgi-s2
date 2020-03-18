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
        List<EventModel> eventList;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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

        public IActionResult Agenda()
        {
            return View();
        }
        [HttpGet]
        public ActionResult CreateEvent()
        {
            return View();
        }
        [HttpPost] 
        public ActionResult CreateEvent(EventModel e)
        {
            if (ModelState.IsValid)
            {
                HandleEventRequest(e);
                return RedirectToAction("Agenda");
            }
            else
            {
                return View(e);
            }
        }
        public IActionResult HandleEventRequest(EventModel emdb)
        {
            string connetionString = "server=185.182.57.161;database=tijnvcd415_Proftaak;uid=tijnvcd415_Proftaak;pwd=Proftaak;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connetionString))
                {
                    //INSERT INTO, UPDATE AND DELETE
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO Rooster (UserId,Subject,Description,Start,End,ThemeColor,IsFullDay,IsPending) VALUES (@UserId,@Subject,@Description,@Start,@End,@ThemeColor,@IsFullDay,@IsPending)", connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", emdb.userId);
                        cmd.Parameters.AddWithValue("@Subject", emdb.title);
                        cmd.Parameters.AddWithValue("@Description", emdb.description);
                        cmd.Parameters.AddWithValue("@Start", emdb.startDate);
                        cmd.Parameters.AddWithValue("@End", emdb.endDate);
                        cmd.Parameters.AddWithValue("@ThemeColor", emdb.themeColor);
                        cmd.Parameters.AddWithValue("@IsFullDay", emdb.isFullDay);
                        cmd.Parameters.AddWithValue("@IsPending", emdb.isPending);
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return RedirectToAction("CreateEvent", "Home");
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
        public IActionResult FetchAllEvents()
        {
            eventList = new List<EventModel>();
            MySqlConnection cnn;
            string connetionString = "server=185.182.57.161;database=tijnvcd415_Proftaak;uid=tijnvcd415_Proftaak;pwd=Proftaak;";
            cnn = new MySqlConnection(connetionString);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = $"select * from Rooster";
            try
            {
                cnn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    EventModel em = new EventModel();
                    em.userId = Convert.ToInt32(reader[1]);
                    em.title = reader[2].ToString();
                    em.description = reader[3].ToString();
                    em.startDate = Convert.ToDateTime(reader[4]);
                    em.endDate = Convert.ToDateTime(reader[5]);
                    em.themeColor = reader[6].ToString();
                    em.isFullDay = Convert.ToBoolean(reader[7]);
                    em.isPending = Convert.ToBoolean(reader[8]);
                    eventList.Add(em);
                }
                cnn.Close();
                return Json(eventList);
            }
            catch (Exception ex)
            {
                return null;
            }
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
