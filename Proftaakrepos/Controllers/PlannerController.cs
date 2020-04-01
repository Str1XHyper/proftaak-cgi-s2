using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Proftaakrepos.Models;

namespace Proftaakrepos.Controllers
{
    public class PlannerController : Controller
    {
        private List<EventModel> eventList;

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult Details()
        {
            return View();
        }
        public IActionResult Agenda()
        {
            return View();
        }
        public IActionResult InitialPlanning(int weeks)
        {
            ViewBag.employees = SQLConnection.ExecuteSearchQuery($"Select Voornaam From Werknemers");
            ViewBag.week = GetWeekDateTimes(weeks);
            ViewBag.weekCount = weeks;
            ViewBag.currentYear = (DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday).AddDays(weeks * 7)).ToString("yyyy");
            return View();
        }
        public static List<DateTime> GetWeekDateTimes(int weeks)
        {
            List<DateTime> currentWeek = new List<DateTime>();
            DateTime startDate = (DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday).AddDays(weeks*7));
            for(int i = 0; i < 7; i++)
            {
                currentWeek.Add((startDate.Date.AddDays(i)));
            }
            return currentWeek;
        }
        [HttpGet]
        public ActionResult CreateEvent()
        {
            string var = HttpContext.Session.GetString("UserInfo");
            string rol = SQLConnection.ExecuteSearchQuery($"Select Rol From Werknemers Where AuthCode = '{var}'")[0];

            ViewBag.Rol = rol;
            var employees = SQLConnection.ExecuteSearchQuery($"Select Voornaam From Werknemers");
            var employeesId = SQLConnection.ExecuteSearchQuery($"Select UserId From Werknemers");
            ViewData["employeesId"] = employeesId.ToArray();
            ViewData["employees"] = employees.ToArray();
            return View();
        }
        [HttpPost]
        public ActionResult CreateEvent(EventModel e)
        {
            if (ModelState.IsValid)
            {
                HandleEventRequest(e);
                return RedirectToAction("CreateEvent");
            }
            else
            {
                return View(e);
            }
        }

        [HttpPost]
        public IActionResult HandleEventRequest(EventModel emdb)
        {
            int userId = 0;
            string var = HttpContext.Session.GetString("UserInfo");
            if (emdb.userId != 0)
            {
                userId = emdb.userId;
            }
            else
            {
                userId = Convert.ToInt32(SQLConnection.ExecuteSearchQuery($"Select UserId From Werknemers Where AuthCode = '{var}'")[0]);
            }
            string rol = SQLConnection.ExecuteSearchQuery($"Select Rol From Werknemers Where AuthCode = '{var}'")[0];
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO Rooster (UserId,Subject,Description,Start,End,ThemeColor,IsFullDay,IsPending) VALUES ('{userId}','{emdb.title}','{emdb.description}','{emdb.startDate.ToString("yyyy/MM/dd HH:mm:ss")}','{emdb.endDate.ToString("yyyy/MM/dd HH:mm:ss")}','{emdb.themeColor}','{Convert.ToInt32(emdb.isFullDay)}','{emdb.isPending}')");
            return RedirectToAction("CreateEvent", "Planner");
        }
        [HttpGet]
        public IActionResult FetchAllEvents(int userId)
        {
            string var = HttpContext.Session.GetString("UserInfo");
            string rol = SQLConnection.ExecuteSearchQuery($"Select Rol From Werknemers Where AuthCode = '{var}'")[0];
            int _userId = 0;
            if (userId == 0)
            {
                _userId = Convert.ToInt32(SQLConnection.ExecuteSearchQuery($"Select UserId From Werknemers Where AuthCode = '{var}'")[0]);
            }
            else
            {
                _userId = userId;
            }
            ViewData["rol"] = rol;
            eventList = new List<EventModel>();

            MySqlConnection cnn;
            string connetionString = "server=185.182.57.161;database=tijnvcd415_Proftaak;uid=tijnvcd415_Proftaak;pwd=Proftaak;";
            cnn = new MySqlConnection(connetionString);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = $"select * from Rooster Where userId = {_userId}";
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

    }
}