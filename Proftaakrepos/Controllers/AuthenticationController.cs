using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Proftaakrepos.Models;

namespace Proftaakrepos.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel lm)
        {
            if (ModelState.IsValid)
            {

            }
            return View(lm);
        }
    }
}