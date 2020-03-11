using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proftaakrepos.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;

namespace Proftaakrepos.Controllers
{
    public class HomeController : Controller
    {
        List<EventModel> mockEvents = new List<EventModel>();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ModelState.AddModelError("", HttpContext.Session.GetString("UserInfo"));
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ShiftView()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
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
        public ActionResult CreateEvent([FromBody]EventModel e)
        {
            if (ModelState.IsValid)
            {
                HandleEventRequest(e);
                return RedirectToAction("Index");
            }
            else
            {
                return View(e);
            }
        }
        public IActionResult HandleEventRequest(EventModel emdb)
        {
            MySqlConnection cnn;
            string connetionString = "server=185.182.57.161;database=tijnvcd415_Proftaak;uid=tijnvcd415_Proftaak;pwd=Proftaak;";
            cnn = new MySqlConnection(connetionString);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = $"INSERT INTO `Rooster`(`EventId`, `UserId`, `Subject`, `Description`, `Start`, `End`, `ThemeColor`, `IsFullDay`) VALUES (5, {emdb.userId}, '{emdb.title}', '{emdb.description}', {emdb.startDate}, {emdb.endDate}, null, 1)";
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

        public IActionResult Block(string UserID, int TradeID, string DisabledIds)
        {
            MySqlConnection cnn;
            string connetionString = "server=185.182.57.161;database=tijnvcd415_Proftaak;uid=tijnvcd415_Proftaak;pwd=Proftaak;";
            cnn = new MySqlConnection(connetionString);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = $"Update TradeRequest Set DisabledIds = '{UserID} {DisabledIds}'Where TradeId = {TradeID}";
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
            string[] returnStrings = new string[9];
            MySqlConnection cnn;
            string connetionString = "server=185.182.57.161;database=tijnvcd415_Proftaak;uid=tijnvcd415_Proftaak;pwd=Proftaak;";
            cnn = new MySqlConnection(connetionString);
            MySqlCommand cmd = new MySqlCommand();
            MySqlCommand cmd2 = new MySqlCommand();
            MySqlCommand cmd3 = new MySqlCommand();
            cmd2.Connection = cnn;
            cmd3.Connection = cnn;
            cmd.Connection = cnn;
            cmd.CommandText = $"Select * from Rooster where EventId = 1";
            try
            {
                cnn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        returnStrings[i] = reader[i].ToString(); //I only used array as an example but you may use built in collections.
                    }
                    break;
                }
                cnn.Close();
                string[] startDates = returnStrings[4].Split(" ")[0].Split("/");
                string startTime = returnStrings[4].Split(" ")[1];
                string[] endDates = returnStrings[5].Split(" ")[0].Split("/");
                string endTime = returnStrings[5].Split(" ")[1];
                Trace.WriteLine($"{startDates[2]}-{startDates[1]}-{startDates[0]} {startTime}");
                cmd2.CommandText = $"Insert Into `TradeRequest`(`UserIdIssuer`, `Status`, `Start`, `End`, `UserIdAcceptor`, `DisabledIDs`) values({UserID}, 0, '{startDates[2]}-{startDates[1]}-{startDates[0]} {startTime}', '{startDates[2]}-{startDates[1]}-{startDates[0]} {startTime}', -1, 0)";

                cnn.Open();
                var reader2 = cmd2.ExecuteNonQuery();
                cnn.Close();
                cmd3.CommandText = $"Update Rooster Set IsPending = 1 where EventId = 2";
                cnn.Open();
                cmd3.ExecuteNonQuery();
                cnn.Close();

            }
            catch (Exception ex)
            {
                //"Can not open connection ! " + ex.Message.ToString()
                Trace.WriteLine(ex);
                return null;
            }
            
            return RedirectToAction("ShiftView", "Home");
        }
    }
}
