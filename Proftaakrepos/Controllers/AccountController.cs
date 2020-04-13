using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace Proftaakrepos.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult ChangeSettings()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            return View();
        }

        public IActionResult LogOut()
        {
            
            HttpContext.Session.Remove("UserInfo");
            return RedirectToAction("Login", "Authentication");
        }
    }
}