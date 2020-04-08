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
    public class HoursWorkedController : Controller
    {
        public IActionResult HoursWorked(int? id)
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            if (id != null)
                //@TODO: Add more logic to Controller.
                return View("HoursWorked");

            return View();
        }
    }
}