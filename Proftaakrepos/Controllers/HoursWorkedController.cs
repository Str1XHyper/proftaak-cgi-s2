using DAL;
using Logic.HoursWorked;
using Logic.Planner;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models;
using Models.HoursWorked;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;

namespace Proftaakrepos.Controllers
{
    public class HoursWorkedController : Controller
    {
        private HoursWorkedModel _overview;
        private readonly TimeSheetManager timeSheetManager;
        private List<HoursWorkedModel> _overviewCollection = new List<HoursWorkedModel>();
        private readonly AgendaManager agendaManager;
        private static string loggedInUserID;
        public HoursWorkedController()
        {
            agendaManager = new AgendaManager();
            timeSheetManager = new TimeSheetManager();
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string language = HttpContext.Session.GetString("Culture");
            if (!string.IsNullOrEmpty(language))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            }
        }
        [HttpGet]
        public IActionResult Index()
        {
            string var = HttpContext.Session.GetString("UserInfo");
            string[] loggedUserData = agendaManager.GetLoggedInUserData(var);
            string rol = loggedUserData[0];
            loggedInUserID = loggedUserData[1];
            return View();
        }
        public string UpdateTable(DateTime Date, string filter)
        {
            string obj = null;
            switch (filter)
            {
                case "week":
                    obj = JsonConvert.SerializeObject(GetWeekData(Date));
                    break;
                case "month":

                    break;
                case "year":

                    break;
            }
            return obj;
        }
        public Week GetWeekData(DateTime Date)
        {
            if (Date.Year==0001) Date = DateTime.Now;
            int selectedWeek = GetIso8601WeekOfYear(Date);
            ViewData["weeknr"] = selectedWeek;
            List<string[]> roosterData = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT * FROM Rooster WHERE Week(Start, 1) = '{selectedWeek}' OR Week(End, 1) = '{selectedWeek}'");
            List<EventModel> eventList = new List<EventModel>();
            foreach (string[] row in roosterData)
            {
                if (row[1] == loggedInUserID)
                {
                    EventModel newEvent = new EventModel()
                    {
                        eventId = Convert.ToInt32(row[0]),
                        userId = row[1],
                        title = row[2],
                        description = row[3],
                        startDate = Convert.ToDateTime(row[4]),
                        endDate = Convert.ToDateTime(row[5]),
                        themeColor = row[6],
                        isFullDay = Convert.ToInt32(row[7]),
                    };
                    eventList.Add(newEvent);
                }
            }
            Week week = new Week();
            foreach (EventModel eventmodel in eventList)
            {
                if (eventmodel.themeColor != "Pauze")
                {
                    if (eventmodel.startDate.DayOfWeek == eventmodel.endDate.DayOfWeek)
                    {
                        if (eventmodel.themeColor == "Stand-by")
                            week = AddHours(eventmodel.startDate.DayOfWeek, week, eventmodel.endDate.Hour - eventmodel.startDate.Hour, "standby");
                        else if (eventmodel.themeColor == "Verlof")
                            week = AddHours(eventmodel.startDate.DayOfWeek, week, eventmodel.endDate.Hour - eventmodel.startDate.Hour, "leave");
                    }
                    else
                    {
                        if (eventmodel.themeColor == "Stand-by")
                        {
                            week = AddHours(eventmodel.startDate.DayOfWeek, week, 24 - eventmodel.startDate.Hour, "standby");
                            week = AddHours(eventmodel.endDate.DayOfWeek, week, eventmodel.endDate.Hour, "standby");
                        }
                        else if (eventmodel.themeColor == "Verlof")
                        {
                            week = AddHours(eventmodel.startDate.DayOfWeek, week, 24 - eventmodel.startDate.Hour, "leave");
                            week = AddHours(eventmodel.endDate.DayOfWeek, week, eventmodel.endDate.Hour, "leave");
                        }
                    }
                }
            }
            return week;
        }
        public Week SeperateHours(string type, Week week, int hours, int day)
        {
            if (type == "incident")
            {
                week.ActiveTime[day] += hours;
                week.Total[day] += hours;
            }
            else if (type == "standby")
            {
                week.StandByTime[day] += hours;
                week.Total[day] += hours;
            }
            else if (type == "leave")
                week.FurloughTime[day] += hours;
            return week;
        }
        public Week AddHours(DayOfWeek day, Week week, int hours, string type)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    week = SeperateHours(type, week, hours, 0);
                    break;
                case DayOfWeek.Tuesday:
                    week = SeperateHours(type, week, hours, 1);
                    break;
                case DayOfWeek.Wednesday:
                    week = SeperateHours(type, week, hours, 2);
                    break;
                case DayOfWeek.Thursday:
                    week = SeperateHours(type, week, hours, 3);
                    break;
                case DayOfWeek.Friday:
                    week = SeperateHours(type, week, hours, 4);
                    break;
                case DayOfWeek.Saturday:
                    week = SeperateHours(type, week, hours, 5);
                    break;
                case DayOfWeek.Sunday:
                    week = SeperateHours(type, week, hours, 6);
                    break;
            }
            return week;
        }
        public int GetIso8601WeekOfYear(DateTime time)
        {
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }


        public IActionResult UpdateIncidents(TimeSheet model)
        {
            List<ParsedTimeSheetRow> timeRows = new List<ParsedTimeSheetRow>();
            for(int i = 0; i < model.Dates.Count; i++)
            {
                ParsedTimeSheetRow row = new ParsedTimeSheetRow();
                row.Start = DateTime.Parse(model.Dates[i]).Add(new TimeSpan(Convert.ToInt32(model.Start[i].Split(':')[0]), Convert.ToInt32(model.Start[i].Split(':')[1]), 0));
                row.Eind = DateTime.Parse(model.Dates[i]).Add(new TimeSpan(Convert.ToInt32(model.End[i].Split(':')[0]), Convert.ToInt32(model.End[i].Split(':')[1]), 0));
                row.Overuren = new TimeSpan(Convert.ToInt32(model.OverTime[i].Split(':')[0]), Convert.ToInt32(model.OverTime[i].Split(':')[1]), 0);
                timeRows.Add(row);
            }
            timeSheetManager.AddNewTimeSheet(timeRows, HttpContext.Session.GetInt32("UserInfo.ID").ToString());
            return RedirectToAction("Index");
        }











        public IActionResult Overview(int? projectId)
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            if (projectId != null)
            {
                List<string> result = SQLConnection.ExecuteSearchQuery($"SELECT Overview.OverviewId, Overview.UserId, WorkDay.StartTime, WorkDay.EndTime, Overview.HoursWorked, Overview.TotalTime, Overview.Overtime FROM ((Overview INNER JOIN WorkDay ON Overview.ProjectId = WorkDay.ProjectId))  WHERE Overview.ProjectId = '{projectId}'");

                for (int i = 0; i <= result.Count; i++)
                {
                    _overviewCollection.Add(_overview = new HoursWorkedModel()
                    {
                        UserId = Convert.ToInt32(result[0]),
                        WeekNumber = Convert.ToInt32(result[1]),
                        BeginTime = Convert.ToDateTime(result[2]),
                        EndTime = Convert.ToDateTime(result[3]),
                        WorkedHours = float.Parse(result[4]),
                        TotalTime = float.Parse(result[5]),
                        Overtime = float.Parse(result[6])
                    });

                    ViewData["Data"] = _overview;
                    result.RemoveRange(0, 7);
                }

                ViewData["Collection"] = _overviewCollection;
                return View("Overview");
            }
            else
            {
                //Make logic/view for when no project id is specified.
                return View();
            }


        }


    }
}