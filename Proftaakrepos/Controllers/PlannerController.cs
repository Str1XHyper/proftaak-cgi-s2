﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Classes;
using ClassLibrary.Planner;
using CookieManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models;
using Models.Agenda;
using Models.Authentication;
using Ubiety.Dns.Core.Records.NotUsed;
using Proftaakrepos.Authorize;

namespace Proftaakrepos.Controllers
{
    public class PlannerController : Controller
    {
        private static string userId;
        private static string rol;
        private AgendaManager agendamanager = new AgendaManager();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TempData["Cookie"] = HttpContext.Session.GetString("UserInfo");
            string language = HttpContext.Session.GetString("Culture");
            if (!string.IsNullOrEmpty(language))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            }
        }
        [UserAccess("","Rooster")]
        public IActionResult Schedule()
        {
            string var = HttpContext.Session.GetString("UserInfo");
            string[] loggedUserData = agendamanager.GetLoggedInUserData(var);
            rol = loggedUserData[0];
            userId = loggedUserData[1];
            HttpContext.Session.SetString("Image", loggedUserData[2]);
            if (rol.ToLower() == "roostermaker")
                ViewData["verlof"] = agendamanager.GetVerlofCount();
            ViewData["rol"] = rol;
            ViewData["userId"] = userId;
            ViewData["language"] = "nl";
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Culture")))
                ViewData["language"] = HttpContext.Session.GetString("Culture");
            return View();
        }
        [UserAccess("", "Rooster")]
        public List<ParseableEventModel> FetchAllEvents(string userIds, string type)
        {
            string[] uids = SterilizeInput(userIds);
            type = HtmlEncoder.Default.Encode(type);

            // Create sql query
            string sqlQuery = $"Select * from `Rooster` WHERE `UserId` = '{uids[0]}'";
            if (type != "all")
            {
                sqlQuery += $"AND `ThemeColor` = '{type}'";
            }
            if (uids.Length > 1)
            {
                for (int i = 1; i < uids.Length; i++)
                {
                    if (!string.IsNullOrEmpty(uids[i]))
                    {
                        sqlQuery += $"OR `UserId` = '{uids[i]}' ";
                        if (type != "all")
                        {
                            sqlQuery += $"AND `ThemeColor` = '{type}'";
                        }
                    }
                }
            }

            // Get names
            List<string[]> names = SQLConnection.ExecuteSearchQueryWithArrayReturn("SELECT `Voornaam`, `Tussenvoegsel`, `Achternaam`, `UserId` FROM `Werknemers`");

            // Get events
            List<string[]> result = SQLConnection.ExecuteSearchQueryWithArrayReturn(sqlQuery);

            // Get colors
            List<string> colors = SQLConnection.ExecuteSearchQuery("SELECT * FROM `ColorScheme`");
            if (colors.Count < 1)
            {
                colors = new List<string>();
                // Standby
                colors.Add("3B5A6F");
                // Incidenten
                colors.Add("353B45");
                // Pauze
                colors.Add("828A87");
                // Verlof
                colors.Add("830101");
            }

            // Put result into models
            String[] c = { "Stand-by", "Incidenten", "Pauze", "Verlof" };
            List<ParseableEventModel> returnList = new List<ParseableEventModel>();
            foreach (string[] row in result)
            {
                int index = Array.IndexOf(c, row[6]);
                string title = row[2];
                if (HttpContext.Session.GetString("Rol").ToLower() == "roostermaker")
                {
                    foreach (string[] name in names)
                    {
                        if (row[1] == name[3])
                            title = name[0] + " - " + title;
                    }
                }

                bool editable = HttpContext.Session.GetString("Rol").ToLower() == "roostermaker";
                ParseableEventModel em = new ParseableEventModel
                {
                    id = Convert.ToInt32(row[0]),
                    title = title,
                    start = DateTime.Parse(row[4]),
                    end = DateTime.Parse(row[5]),
                    backgroundColor = "#" + colors[index],
                    allDay = Convert.ToBoolean(Convert.ToInt32(row[7])),
                    description = row[3],
                    borderColor = "#010203",
                    soort = row[6],
                    userId = row[1],
                    editable = editable,
                };
                returnList.Add(em);
            }

            // return parsable model for Full Calendar
            return returnList;
        }
        [UserAccess("", "Rooster wijzigen")]
        public void UpdateEvent(DateTime start, DateTime end, string eventid, bool allday)
        {
            // Encode to prevent SQL Injection
            string startTime = HtmlEncoder.Default.Encode(start.ToString("yyyy/MM/dd HH:mm"));
            string endTime = HtmlEncoder.Default.Encode(end.ToString("yyyy/MM/dd HH:mm"));
            eventid = HtmlEncoder.Default.Encode(eventid);
            string alldaystring = HtmlEncoder.Default.Encode(allday.ToString());
            allday = Boolean.Parse(alldaystring);

            // Update event
            SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set Start = '{startTime}',End = '{endTime}',IsFullDay = '{Convert.ToInt32(allday)}' Where EventId = '{eventid}'");
        }
        //[UserAccess("", "Rooster wijzigen")]
        [HttpPost]
        public void CreateEvent(EventModel newmodel)
        {
            if (!string.IsNullOrEmpty(newmodel.userId))
            {
                if (newmodel.userId.EndsWith(",")) newmodel.userId = newmodel.userId.Substring(0, newmodel.userId.Length - 1);
                // Get selected userIDs
                string[] uids = SterilizeInput(newmodel.userId);

                // Create sql query
                string sqlquery = $"INSERT INTO Rooster(UserId, Subject, Description, Start, End, ThemeColor, IsFullDay, IsPending) VALUES ";
                for (int i = 0; i < uids.Length; i++)
                {
                    if (i > 0 && !string.IsNullOrEmpty(uids[i]))
                        sqlquery += ",";
                    if (!string.IsNullOrEmpty(uids[i]))
                        sqlquery += $"('{uids[i]}', '{newmodel.title}', '{newmodel.description}', '{newmodel.startDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{newmodel.endDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{newmodel.themeColor}', '{(newmodel.isFullDay)}', '{(newmodel.isPending ? 1 : 0)}')";
                }
                SQLConnection.ExecuteNonSearchQuery(sqlquery);

                // When an event has type "Verlof" it creates a new Absence request
                string eventID = SQLConnection.ExecuteSearchQuery($"SELECT MAX(EventId) FROM Rooster")[0];
                if (newmodel.themeColor.ToLower() == "verlof")
                {
                    SQLConnection.ExecuteNonSearchQuery($"INSERT INTO Verlofaanvragen (UserID, EventID) VALUES ('{userId}', '{eventID}')");
                }
                else
                {
                    NotificationSettings settings = new NotificationSettings();
                    settings.SendRoosterNotifcation(newmodel.userId, eventID);
                }
            }
        }
        [UserAccess("", "Rooster wijzigen")]
        public string[] GetUsers()
        {
            // Get names from database
            List<string[]> names = SQLConnection.ExecuteSearchQueryWithArrayReturn("SELECT `UserId`, `Voornaam`, `Tussenvoegsel`, `Achternaam` FROM `Werknemers` ORDER BY `UserId` ASC");

            // Put names in correct format
            string[] parsedNames = new string[names.Count];
            for (int i = 0; i < names.Count; i++)
            {
                string completeName = "";
                foreach (string namePiece in names[i])
                {
                    completeName += namePiece + " ";
                }
                completeName.Trim();
                parsedNames[i] = completeName;
            }

            // Return name array.
            return parsedNames;
        }

        ///<summary>
        ///Sterilize string input seperated with ','
        ///return string[] of valid and clean data
        ///</summary>
        private string[] SterilizeInput(string input)
        {
            // Check if input is not empty
            if (input == "" || input == null)
            {
                throw new ArgumentNullException();
            }

            // Encode to prevent SQL Injection
            string[] cleanData = new string[input.Split(',').Length];
            for (int i = 0; i < input.Split(",").Length; i++)
            {
                cleanData[i] = HtmlEncoder.Default.Encode(input.Split(',')[i]);
            }

            return cleanData;
        }
        public void DeleteEvent(int EventId)
        {
            List<string[]> doesExists = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT * FROM `Verlofaanvragen` WHERE `EventID`='{EventId}'");
            if (doesExists.Count > 0)
            {
                SQLConnection.ExecuteNonSearchQuery($"DELETE FROM Verlofaanvragen WHERE EventId = {EventId}");
            }
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM Rooster WHERE EventId = {EventId}");
        }
    }
}