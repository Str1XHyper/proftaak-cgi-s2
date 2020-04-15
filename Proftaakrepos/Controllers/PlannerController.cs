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
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return View();
        }
        public IActionResult Create()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return View();
        }
        public IActionResult Delete()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return View();
        }
        public IActionResult Edit()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return View();
        }
        public IActionResult Details()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return View();
        }
        public IActionResult Agenda()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            string var = HttpContext.Session.GetString("UserInfo");
            string rol = SQLConnection.ExecuteSearchQuery($"Select Rol From Werknemers Where AuthCode = '{var}'")[0];
            var userId = SQLConnection.ExecuteSearchQuery($"Select UserId From Werknemers Where AuthCode = '{var}'")[0];
            var employees = SQLConnection.ExecuteSearchQuery($"Select Voornaam From Werknemers");
            var employeesId = SQLConnection.ExecuteSearchQuery($"Select UserId From Werknemers");
            ViewData["employeesId"] = employeesId.ToArray();
            ViewData["employees"] = employees.ToArray();
            ViewData["rol"] = rol;
            ViewData["userId"] = userId;
            return View();
        }
        public IActionResult InitialPlanning(int weeks)
        {
            ViewBag.employees = SQLConnection.ExecuteSearchQuery($"Select Voornaam From Werknemers");
            ViewBag.week = GetWeekDateTimes(weeks);
            ViewBag.weekCount = weeks;
            ViewBag.currentYear = (DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday).AddDays(weeks * 7)).ToString("yyyy");
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
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
        public void DeleteEvent(int EventId)
        {
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM Rooster WHERE EventId = {EventId}");
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
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return View();

        }
        public ActionResult GetEventInfo(int EventId)
        {
            List<string> eventData = SQLConnection.ExecuteSearchQuery($"select * from Rooster Where EventId = {EventId}");
            var start = DateTime.Parse(eventData[4]);
            var end = DateTime.Parse(eventData[5]);
            eventData[4] = start.ToString("yyyy-MM-dd'T'HH:mm");
            eventData[5] = end.ToString("yyyy-MM-dd'T'HH:mm");
            return Json(eventData);
        }

        [HttpPost]
        public IActionResult EditEvent(EventModel e, string pagename)
        {
            if (ModelState.IsValid)
            {
                HandleEditEventRequest(e);
            }
            if(pagename == "CreateEvent")
            {
                return RedirectToAction("CreateEvent");
            }
            return RedirectToAction("Agenda");
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
        public void HandleEditEventRequest(EventModel emdb)
        {
            SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set UserId = '{emdb.userId}',Subject = '{emdb.title}', Description = '{emdb.description}', Start = '{emdb.startDate.ToString("yyyy/MM/dd HH:mm")}', End = '{emdb.endDate.ToString("yyyy/MM/dd HH:mm")}', ThemeColor = '{emdb.themeColor}', IsFullDay = '{Convert.ToInt32(emdb.isFullDay)}' Where EventId = '{emdb.eventId}'");
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
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO Rooster (UserId,Subject,Description,Start,End,ThemeColor,IsFullDay,IsPending) VALUES ('{userId}','{emdb.title}','{emdb.description}','{emdb.startDate.ToString("yyyy/MM/dd HH:mm:ss")}','{emdb.endDate.ToString("yyyy/MM/dd HH:mm:ss")}','{emdb.themeColor}','{Convert.ToInt32(emdb.isFullDay)}','{(emdb.isPending ? 1 : 0)}')");
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

            List<string[]> events = SQLConnection.ExecuteSearchQueryWithArrayReturn($"select * from Rooster Where userId = {_userId}");

            foreach (string[] e in events)
            {
                EventModel em = new EventModel();
                em.eventId = Convert.ToInt32(e[0]);
                em.userId = Convert.ToInt32(e[1]);
                em.title = e[2].ToString();
                em.description = e[3].ToString();
                em.startDate = Convert.ToDateTime(e[4]);
                em.endDate = Convert.ToDateTime(e[5]);
                em.themeColor = e[6].ToString();
                em.isFullDay = Convert.ToBoolean(e[7]);
                em.isPending = Convert.ToBoolean(e[8]);
                eventList.Add(em);
            }
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return Json(eventList);
        }
        public void UpdateAgendaTimes(DateTime startTime, DateTime endTime, int EventId)
        {
            SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set Start = '{startTime.ToString("yyyy/MM/dd HH:mm")}',End = '{endTime.ToString("yyyy/MM/dd HH:mm")}' Where EventId = {EventId}");
        }

    }
}