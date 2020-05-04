using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Proftaakrepos.Controllers
{
    [Route("error")]
    public class ErrorController : Controller
    {
        [Route("500")]
        public IActionResult Error500()
        {
            return View();
        }

        [Route("401")]
        public IActionResult Error401()
        {
            return View();
        }
    }
}