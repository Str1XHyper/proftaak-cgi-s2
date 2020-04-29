using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Proftaakrepos.Models;

namespace Proftaakrepos.Controllers
{
    public class PlannerController : Controller
    {

        private static string userId;
        private static string rol;
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
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return View();
        }
        public IActionResult Agenda()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            string var = HttpContext.Session.GetString("UserInfo");
            rol = SQLConnection.ExecuteSearchQuery($"Select Rol From Werknemers Where AuthCode = '{var}'")[0];
            userId = SQLConnection.ExecuteSearchQuery($"Select UserId From Werknemers Where AuthCode = '{var}'")[0];
            //var employees = SQLConnection.ExecuteSearchQuery($"Select Voornaam From Werknemers");
            //var employeesId = SQLConnection.ExecuteSearchQuery($"Select UserId From Werknemers");
            //ViewData["employeesId"] = employeesId.ToArray();
            //ViewData["employees"] = employees.ToArray();
            ViewData["rol"] = rol;
            ViewData["userId"] = userId;
            AgendaViewModel viewdata = new AgendaViewModel();
            string[] roosterData = SQLConnection.ExecuteSearchQuery($"Select Rooster.*, Werknemers.Voornaam From Rooster INNER JOIN Werknemers ON Werknemers.UserId = Rooster.UserId").ToArray();
            List<EventModel> modelList = new List<EventModel>();
            for (int i = 0; i < roosterData.Length; i+=10){
                EventModel model = new EventModel();
                model.eventId = Convert.ToInt32(roosterData[i]);
                model.userId = roosterData[i+1];
                model.title = roosterData[i+2];
                model.description = roosterData[i+3];
                model.startDate = Convert.ToDateTime(roosterData[i+4]);
                model.endDate = Convert.ToDateTime(roosterData[i+5]);
                model.themeColor = roosterData[i+6];
                model.isFullDay = Convert.ToInt32(roosterData[i+7]);
                model.voornaam = roosterData[i+9];
                viewdata.eventList.Add(model);
            }
            string[] userData = SQLConnection.ExecuteSearchQuery($"Select UserId, Voornaam, Tussenvoegsel, Achternaam, Rol From Werknemers").ToArray();
            for (int i = 0; i < userData.Length; i += 5)
            {
                UserViewModel usermodel = new UserViewModel(userData[i], userData[i + 1], userData[i + 2], userData[i + 3], userData[i + 4]);
                viewdata.userList.Add(usermodel);
            }
            string _authCode = HttpContext.Session.GetString("UserInfo");
            if (CheckIfAllowed.IsAllowed(_authCode, "Agenda")) return View(viewdata);
            else return RedirectToAction("NoAccessIndex", "Home");
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
            DateTime startDate = (DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday).AddDays(weeks * 7));
            for (int i = 0; i < 7; i++)
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
        public ActionResult DatePicker()
        {
            return View();
        }
        public ActionResult GetEventInfo(int EventId)
        {
            List<string> eventData = SQLConnection.ExecuteSearchQuery($"select Rooster.*, Werknemers.Voornaam from Rooster INNER JOIN Werknemers ON Werknemers.UserId = Rooster.UserId Where EventId = {EventId}");
            var start = DateTime.Parse(eventData[4]);
            var end = DateTime.Parse(eventData[5]);
            eventData[4] = start.ToString("yyyy-MM-dd'T'HH:mm");
            eventData[5] = end.ToString("yyyy-MM-dd'T'HH:mm");
            return Json(eventData);
        }

        [HttpPost]
        public IActionResult EditEvent(EventModel e)
        {
            if (ModelState.IsValid)
            {
                HandleEditEventRequest(e);
            }
            return RedirectToAction("Agenda");
        }
        [HttpPost]
        public void CreateEvent(EventModel newmodel)
        {
            newmodel.userId = newmodel.userId.Substring(0, newmodel.userId.Length - 1);
            string[] userIdArray;
            userIdArray = newmodel.userId.Split(",");
            if (ModelState.IsValid)
            {
                if (newmodel.eventId > 0)
                {
                    HandleEditEventRequest(newmodel);
                }
                else
                {
                    HandleEventRequest(newmodel, userIdArray);
                }
            }
            //return RedirectToAction("Agenda");
        }
        public void HandleEditEventRequest(EventModel emdb)
        {
            emdb.userId = emdb.userId.Substring(0, emdb.userId.Length);
            string[] userIdArray = emdb.userId.Split(",");
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM Rooster WHERE EventId = {emdb.eventId}");
            //SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set UserId = '{emdb.userId}',Subject = '{emdb.title}', Description = '{emdb.description}', Start = '{emdb.startDate.ToString("yyyy/MM/dd HH:mm")}', End = '{emdb.endDate.ToString("yyyy/MM/dd HH:mm")}', ThemeColor = '{emdb.themeColor}', IsFullDay = '{Convert.ToInt32(emdb.isFullDay)}' Where EventId = '{emdb.eventId}'");
            string sqlquery = $"INSERT INTO Rooster(UserId, Subject, Description, Start, End, ThemeColor, IsFullDay, IsPending) VALUES ";
            for (int i = 0; i < userIdArray.Length; i++)
            {
                if (i > 0)
                {
                    sqlquery += ",";
                }
                sqlquery += $"('{userIdArray[i]}', '{emdb.title}', '{emdb.description}', '{emdb.startDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{emdb.endDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{emdb.themeColor}', '{(emdb.isFullDay)}', '{(emdb.isPending ? 1 : 0)}')";
            }
            SQLConnection.ExecuteNonSearchQuery(sqlquery);
        }
        [HttpPost]
        public IActionResult HandleEventRequest(EventModel emdb, string[] useridArray)
        {
            string userId = "0";
            string var = HttpContext.Session.GetString("UserInfo");
            if (emdb.userId != "0")
            {
                userId = emdb.userId;
            }
            else
            {
                
            }
            if (userId == "-1")
            {
                int userCount = Convert.ToInt32(SQLConnection.ExecuteSearchQuery($"Select Count(UserId) From Werknemers")[0]);
                for(int i = 0; i < userCount; i++)
                {
                    SQLConnection.ExecuteNonSearchQuery($"INSERT INTO Rooster (UserId,Subject,Description,Start,End,ThemeColor,IsFullDay,IsPending) VALUES ('{i}','{emdb.title}','{emdb.description}','{emdb.startDate.ToString("yyyy/MM/dd HH:mm:ss")}','{emdb.endDate.ToString("yyyy/MM/dd HH:mm:ss")}','{emdb.themeColor}','{(emdb.isFullDay)}','{(emdb.isPending ? 1 : 0)}')");
                }
            }
            else
            {
                string sqlquery = $"INSERT INTO Rooster(UserId, Subject, Description, Start, End, ThemeColor, IsFullDay, IsPending) VALUES ";
                for(int i = 0; i < useridArray.Length; i++)
                {
                    if (i > 0)
                    {
                        sqlquery += ",";
                    }
                    sqlquery += $"('{useridArray[i]}', '{emdb.title}', '{emdb.description}', '{emdb.startDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{emdb.endDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{emdb.themeColor}', '{(emdb.isFullDay)}', '{(emdb.isPending ? 1 : 0)}')";
                }
                SQLConnection.ExecuteNonSearchQuery(sqlquery);
            }
            return RedirectToAction("CreateEvent", "Planner");
        }
        [HttpGet]
        public IActionResult FetchAllEvents(string SendUserId)
        {
            string var = HttpContext.Session.GetString("UserInfo");
            string _userId = "0";
            string[] userIdArray = new string[0];
            if (SendUserId == null || SendUserId == "0")
            {
                _userId = userId;
            }
            else
            {
                SendUserId = SendUserId.Substring(0, SendUserId.Length - 1);
                userIdArray = SendUserId.Split(",");
            }
            ViewData["rol"] = rol;
            eventList = new List<EventModel>();
            List<string[]> events;
            if (SendUserId == "-1")
            {
                events = SQLConnection.ExecuteSearchQueryWithArrayReturn($"select Rooster.*, Werknemers.Voornaam from Rooster INNER JOIN Werknemers ON Rooster.UserId = Werknemers.UserId");
            }
            else if(SendUserId== "0")
            {
                events = SQLConnection.ExecuteSearchQueryWithArrayReturn($"select * from Rooster WHERE UserId = '{_userId}'");
            }
            else
            {
                string sqlquery = $"select Rooster.*, Werknemers.Voornaam from Rooster INNER JOIN Werknemers ON Rooster.UserId = Werknemers.UserId";
                //events = SQLConnection.ExecuteSearchQueryWithArrayReturn($"select * from Rooster Where UserId = {_userId}");
                for (int i = 0; i < userIdArray.Length; i++)
                {
                    if(i == 0)
                    {
                        sqlquery += " WHERE ";
                    }
                    if (i > 0)
                    {
                        sqlquery += " OR ";
                    }
                    sqlquery += $"Rooster.UserId = '{userIdArray[i]}'";
                }
                
                events = SQLConnection.ExecuteSearchQueryWithArrayReturn(sqlquery);
            }
            foreach (string[] e in events)
            {
                EventModel em = new EventModel();
                em.eventId = Convert.ToInt32(e[0]);
                em.userId = e[1];
                if (SendUserId == "-1" || SendUserId == null || rol == "Roostermaker")
                {
                    em.title = e[9] + " - " + e[2].ToString();
                }
                else
                {
                    em.title = e[2].ToString();
                }
                em.description = e[3].ToString();
                em.startDate = Convert.ToDateTime(e[4]);
                em.endDate = Convert.ToDateTime(e[5]);
                em.themeColor = e[6].ToString();
                em.isFullDay = Convert.ToInt32(e[7]);
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