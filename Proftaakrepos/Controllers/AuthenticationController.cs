﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Proftaakrepos.Models;
using System;
using System.Threading.Tasks;
using ClassLibrary;
using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Proftaakrepos.Controllers
{
    public class AuthenticationController : Controller
    {

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            string response =  LoginClass.LoginUserFunction(model.Username, model.Password).ToString();
            switch (response)
            {
                case "redirectHome":
                    string authCode = CreateLoginCookie.getAuthToken(model.Username);
                    HttpContext.Session.SetString("UserInfo", authCode);
                    AddLogin(true, model.Username);
                    return RedirectToAction("Agenda", "Planner");
                case "wrongEntry":
                    ViewData["Error"] = "Verkeerde e-mail of wachtwoord combinatie.";
                    AddLogin(false, model.Username);
                    break;
                case "multipleEntries":
                    ViewData["Error"] = "Meerdere accounts gevonden met dit e-mail.";
                    break;
                case "massiveError":
                    ViewData["Error"] = "Godverdomme Bart, hoe moeilijk is het?";
                    break;

            }
            return View(model);
        }

        public void AddLogin(bool success, string username)
        {
            AddLoginLog addLoginLog = new AddLoginLog();
            string authCode = CreateLoginCookie.getAuthToken(username);
            string timeNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            string userIP = Response.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString();
            addLoginLog.NewLogin(authCode, success, userIP, timeNow);
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
            return View();
        }

        [HttpPost]
        public IActionResult ChangePasswordPost(ChangePassword weirdflex)
        {
            AddLoginAccount.ChangeLoginAdmin(weirdflex.email, weirdflex.password);
            return View("ChangePassword");
        }

        public IActionResult LoginNew()
        {
            if (HttpContext.Session.GetString("UserInfo") != null)
            {
                return RedirectToAction("Agenda", "Planner");
            }
            return View();
        }
    }
}