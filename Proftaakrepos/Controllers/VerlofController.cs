using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Proftaakrepos.Controllers
{
    public class VerlofController : Controller
    {
        // GET: Verlof
        [HttpGet]
        public ActionResult Aanvragen()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RequestLeave()
        {
            return View("Aanvragen");
        }
    }
}