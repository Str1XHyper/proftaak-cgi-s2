using DAL;
using FullCalendar.Infrastructure.PropertyParsers;
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
        private readonly EventManager eventmanger;
        private readonly OverviewTableManager tablemanager;
        private static string loggedInUserID;
        public HoursWorkedController()
        {
            agendaManager = new AgendaManager();
            timeSheetManager = new TimeSheetManager();
            tablemanager = new OverviewTableManager();
            eventmanger = new EventManager();
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
            int selectedWeek = tablemanager.GetIso8601WeekOfYear(Date);
            ViewData["weeknr"] = selectedWeek;
            List<string[]> timesheetEventList = tablemanager.GetTimesheetEventsByWeek(selectedWeek);
            List<EventModel> eventList = tablemanager.AssembleEventModel(timesheetEventList, loggedInUserID);
            Week week = tablemanager.AssembleWeekModel(eventList);
            return week;
        }
        
        
        
        public IActionResult UpdateIncidents(TimeSheet model)
        {
            List<ParsedTimeSheetRow> timeRows = new List<ParsedTimeSheetRow>();
            for(int i = 0; i < model.Dates.Count; i++)
            {
                ParsedTimeSheetRow row = new ParsedTimeSheetRow();
                try
                {
                    row.Start = DateTime.Parse(model.Dates[i]).Add(new TimeSpan(Convert.ToInt32(model.Start[i].Split(':')[0]), Convert.ToInt32(model.Start[i].Split(':')[1]), 0));
                    row.Eind = DateTime.Parse(model.Dates[i]).Add(new TimeSpan(Convert.ToInt32(model.End[i].Split(':')[0]), Convert.ToInt32(model.End[i].Split(':')[1]), 0));
                    row.Overuren = new TimeSpan(Convert.ToInt32(model.OverTime[i].Split(':')[0]), Convert.ToInt32(model.OverTime[i].Split(':')[1]), 0);
                    row.Type = model.Type[i] == "on" ? "Incidenten" : "Stand-by";
                }
                catch
                {
                    return RedirectToAction("Index");
                }
                timeRows.Add(row);
            }
            timeSheetManager.AddNewTimeSheet(timeRows, HttpContext.Session.GetInt32("UserInfo.ID").ToString());
            return RedirectToAction("Index");
        }

        
        /*public IActionResult Overview(int? projectId)
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


        }*/


    }
}