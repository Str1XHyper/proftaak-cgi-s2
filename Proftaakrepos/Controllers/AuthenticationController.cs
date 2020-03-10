using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Proftaakrepos.Models;
using System;
using System.Threading.Tasks;
using Proftaakrepos.Data;
using ClassLibrary;
using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;


namespace Proftaakrepos.Controllers
{
    public class AuthenticationController : Controller
    {

        private CreateLoginCookie createLoginCookie = new CreateLoginCookie();
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            string response =  LoginClass.LoginUserFunction(model.Username, model.Password).ToString();

            switch (response)
            {
                case "redirectHome":
                    //createLoginCookie.CreateCookie(model.Username);
                    string authCode = createLoginCookie.getAuthToken(model.Username);
                    HttpContext.Session.SetString("UserInfo", authCode);
                    return RedirectToAction("Index", "Home");
                    break;
                case "wrongEntry":
                    ModelState.AddModelError("", "Wrong e-mail or password.");
                    break;
                case "multipleEntries":
                    ModelState.AddModelError("", "Multiple emails found, please contact a system administrator.");
                    break;
                case "massiveError":
                    ModelState.AddModelError("", "Godverdomme Bart, hoe moeilijk is het?");
                    break;

            }
            return View(model);
        }

        public IActionResult AddEmployee()
        {
            string authCode = "738465773";
            HttpContext.Session.SetString("UserInfo", authCode);
            return View();
        }

    }
}