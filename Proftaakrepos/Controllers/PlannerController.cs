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
            string connetionString = "server=185.182.57.161;database=tijnvcd415_Proftaak;uid=tijnvcd415_Proftaak;pwd=Proftaak;";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connetionString))
                {
                    //INSERT INTO, UPDATE AND DELETE
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO Rooster (UserId,Subject,Description,Start,End,ThemeColor,IsFullDay,IsPending) VALUES (@UserId,@Subject,@Description,@Start,@End,@ThemeColor,@IsFullDay,@IsPending)", connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
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
            userId = 0;
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