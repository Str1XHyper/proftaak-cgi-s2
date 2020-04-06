using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Proftaakrepos.Controllers
{
    public class HoursWorkedController : Controller
    {
        public IActionResult HoursWorked(int? id)
        {
            if (id != null)
                //@TODO: Add more logic to Controller.
                return View("HoursWorked");
        }
    }
}