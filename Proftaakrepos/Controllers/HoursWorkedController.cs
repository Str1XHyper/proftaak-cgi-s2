using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proftaakrepos.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using ClassLibrary.Classes;
using System.Reflection;

namespace Proftaakrepos.Controllers
{
    public class HoursWorkedController : Controller
    {
        private HoursWorkedModel _overview;
        private List<HoursWorkedModel> _overviewCollection = new List<HoursWorkedModel>();
        public IActionResult Index()
        {
            return View("Overview");
        }

        public IActionResult Overview(int? projectId) //add user id param here pls :)
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            if (projectId != null)
            {
                List<string> result = SQLConnection.ExecuteSearchQuery($"SELECT `UserId`, `WeekNumber`, `BeginTime`, `EndTime`, `HoursWorked`, `TotalTime`, `Overtime` FROM `Overview` WHERE `ProjectId` = '{projectId}'");

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

            return View();
        }


    }
}