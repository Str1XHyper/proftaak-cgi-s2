using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Classes;

namespace Proftaakrepos.Controllers
{
    public class EmployeesController : Controller
    {
        private SQLConnection sql = new SQLConnection();
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetEmployeeInfo(string employee)
        {
            string[] data = sql.ExecuteSearchQuery($"Select * from `Werknemers` where UserId = {employee}").ToArray();
            ViewData["EmployeeInfo"] = data;
            return RedirectToAction("Employees", "Authentication");
        }
    }
}