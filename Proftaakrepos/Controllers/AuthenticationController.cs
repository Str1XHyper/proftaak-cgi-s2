using ClassLibrary;
using ClassLibrary.Classes;
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
                Date = DateTime.Now
            };

            string response = LoginClass.LoginUserFunction(model.Username, model.Password).ToString();
            switch (response)
            {
                case "redirectHome":
                    string authCode = CreateLoginCookie.getAuthToken(model.Username);
                    AddLogin(true, model.Username, model.IP);
                    cookie.Identifier = authCode;
                    if (model.Remember) _cookieManager.Set("BIER.User", cookie, 30 * 1440);
                    HttpContext.Session.SetString("UserInfo", authCode);
                    return RedirectToAction("Agenda", "Planner");
                case "wrongEntry":
                    ViewData["Error"] = "Verkeerde e-mail of wachtwoord combinatie.";
                    AddLogin(false, model.Username, model.IP);
                    break;
                case "multipleEntries":
                    ViewData["Error"] = "Meerdere accounts gevonden met dit e-mail.";
                    break;
                case "massiveError":
                    ViewData["Error"] = "Godverdomme Bart, hoe moeilijk is het?";
                    break;

            }
            return View("LoginNew");
        }

        public void AddLogin(bool success, string username, string ip)
        {
            AddLoginLog addLoginLog = new AddLoginLog();
            string authCode = CreateLoginCookie.getAuthToken(username);
            string timeNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            addLoginLog.NewLogin(authCode, success, ip,  timeNow);
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePassword changePassword)
        {
            AddLoginAccount.ChangeLoginAdmin(changePassword.email, changePassword.password);
            ViewData["msg"] = "Wachtwoord aangepast";
            return View("Employees");
        }

        public IActionResult ChangePassword()
        {
            TempData["CookieMonster"] = _cookieManager.Get<CookieModel>("BIER.User");
            return View();
        }

        [HttpPost]
        public IActionResult ChangePasswordPost(ChangePassword weirdflex)
        {
            AddLoginAccount.ChangeLoginAdmin(weirdflex.email, weirdflex.password);
            return View("ChangePassword");
        }
        public IActionResult LoginNew(string extra)
        {
            if (HttpContext.Session.GetString("UserInfo") != null)
            {
                return RedirectToAction("Agenda", "Planner");
            }
            if (_cookieManager.Get<CookieModel>("BIER.User") != null)
            {
                HttpContext.Session.SetString("UserInfo", _cookieManager.Get<CookieModel>("BIER.User").Identifier);
                return RedirectToAction("Agenda", "Planner");
            }
            if (extra != null)
            {
                ViewData["Error"] = "Succesvol uitgelogd.";
            }
            return View();
        }

        [HttpPost]
        public ActionResult<CheckPasswordModel> CheckPassword(string password)
        {
            List<string> result = SQLConnection.ExecuteSearchQuery($"SELECT * FROM `PasswordRequirements`");
            CheckPasswordModel cpm = new CheckPasswordModel(Convert.ToBoolean(Convert.ToInt16(result[0])), Convert.ToBoolean(Convert.ToInt16(result[1])), Convert.ToBoolean(Convert.ToInt16(result[2])), Convert.ToBoolean(Convert.ToInt16(result[3])), Convert.ToInt32(result[4]));
            PasswordCheck passwordCheck = new PasswordCheck(cpm, HttpContext.Session.GetString("UserInfo"));
            return passwordCheck.CheckPassword(password);
        }
    }
}