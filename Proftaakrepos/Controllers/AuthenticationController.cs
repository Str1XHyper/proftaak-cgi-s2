using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Authentication;
using Models;
using Proftaakrepos.Authorize;
using System;
using System.Collections.Generic;
using System.Net;
using CookieManager;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading;
using System.Globalization;
using Models.Language;
using Logic.Authentication.Login;
using Logic.Employees;
using Logic.Authentication.Password;
using Logic.Authentication;
using DAL;
using Logic;

namespace Proftaakrepos.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ICookieManager _cookieManager;
        private readonly ICookie _cookie;

        public AuthenticationController(ICookieManager cookieManager, ICookie cookie)
        {
            this._cookieManager = cookieManager;
            this._cookie = cookie;

        }
        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            CookieModel cookie = new CookieModel()
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = null,
                Date = DateTime.Now,
                Role = null
            };
            LoginManager loginManager = new LoginManager();
            string response = loginManager.LoginUser(model.Username, model.Password).ToString();
            EmployeeInfoManager employeeInfo = new EmployeeInfoManager();
            GetAccesLevel accessLevel = new GetAccesLevel();
            switch (response)
            {
                case "redirectHome":
                    string authCode = employeeInfo.getAuthToken(model.Username);
                    AddLogin(true, model.Username, model.IP);
                    cookie.Identifier = authCode;
                    cookie.Role = accessLevel.GetRol(authCode);
                    if (model.Remember) _cookieManager.Set("BIER.User", cookie, 30 * 1440);
                    SetSession(authCode);
                    return RedirectToAction("Schedule", "Planner");
                case "wrongEntry":
                    ViewData["Error"] = "Verkeerde e-mail of wachtwoord combinatie.";
                    AddLogin(false, model.Username, model.IP);
                    return View("LoginNew");
                case "multipleEntries":
                    ViewData["Error"] = "Meerdere accounts gevonden met dit e-mail.";
                    return View("LoginNew");
                case "massiveError":
                    ViewData["Error"] = "Godverdomme Bart, hoe moeilijk is het?";
                    return View("LoginNew");
                default:
                    return View("ChangePassword");

            }
        }

        private void SetSession(string authCode)
        {
            NotificationManager nm = new NotificationManager();
            nm.SendNotifications();
            EmployeeInfoManager employeeInfo = new EmployeeInfoManager();
            HttpContext.Session.SetString("UserInfo", authCode);
            List<string> UInfo = employeeInfo.EmployeeInfo(authCode);
            string UserID = UInfo[0];
            string Name = string.Empty;
            string rol = UInfo[11];
            if (UInfo[2] != string.Empty) Name = UInfo[1] + " " + UInfo[2] + " " + UInfo[3];
            else Name = UInfo[1] + " " + UInfo[3];
            HttpContext.Session.SetInt32("UserInfo.ID", Convert.ToInt32(UserID));
            HttpContext.Session.SetString("UserInfo.Name", Name);
            if(_cookieManager.Get<LanguageCookieModel>("BIER.User.Culture") != null) HttpContext.Session.SetString("Culture", _cookieManager.Get<LanguageCookieModel>("BIER.User.Culture").Language);
            else HttpContext.Session.SetString("Culture", "en");
            if (_cookieManager.Get<CookieModel>("BIER.User") != null) HttpContext.Session.SetString("Rol", _cookieManager.Get<CookieModel>("BIER.User").Role);
            else HttpContext.Session.SetString("Rol", UInfo[11]);

        }

        public void AddLogin(bool success, string username, string ip)
        {
            EmployeeInfoManager employeeInfo = new EmployeeInfoManager();
            string authCode = employeeInfo.getAuthToken(username);
            LoginManager loginManager = new LoginManager();
            loginManager.AddLoginRecord(authCode, success, ip, DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePassword changePassword)
        {
            LoginManager loginManager = new LoginManager();
            loginManager.ChangePasswordAdmin(changePassword.email, changePassword.password);
            ViewData["msg"] = "Wachtwoord aangepast";
            return View("Employees");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePasswordPost(ChangePassword model)
        {
            LoginManager loginManager = new LoginManager();
            loginManager.ChangePasswordAdmin(model.email, model.password);
            return View("ChangePassword");
        }
        [HttpGet]
        public IActionResult LoginNew(string extra)
        {
            if (HttpContext.Session.GetString("UserInfo") != null)
            {
                return RedirectToAction("Schedule", "Planner");
            }
            if (_cookieManager.Get<CookieModel>("BIER.User") != null)
            {
                SetSession(_cookieManager.Get<CookieModel>("BIER.User").Identifier);
                return RedirectToAction("Schedule", "Planner");
            }
            if (_cookieManager.Get<LanguageCookieModel>("BIER.User.Culture") != null) HttpContext.Session.SetString("Culture", _cookieManager.Get<LanguageCookieModel>("BIER.User.Culture").Language);
            else HttpContext.Session.SetString("Culture", "en");
            if (extra != null)
            {
                ViewData["Error"] = extra;
            }
            return View();
        }
        public IActionResult Login()
        {
            return RedirectToAction("LoginNew");
        }

        [HttpPost]
        public ActionResult<CheckPasswordModel> CheckPassword(string password)
        {
            PasswordRequirements passwordCheck = new PasswordRequirements();
            return passwordCheck.CheckPassword(password);
        }
    }
}