﻿using ClassLibrary.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Proftaakrepos.Authorize;
using System;

namespace Proftaakrepos.Controllers
{
    public class AccountController : Controller
    {
        [UserAccess("LoggedIn", "")]
        public IActionResult ChangeSettings()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            string[] data = SQLConnection.ExecuteSearchQuery($"Select * from `Werknemers` where authCode = '{HttpContext.Session.GetString("UserInfo")}'").ToArray();
            ViewData["userInformation"] = data;
            data = SQLConnection.ExecuteSearchQuery($"SELECT * FROM `Settings` WHERE UserId = '{data[0]}'").ToArray();
            ViewData["Settings"] = data;
            return View();
        }

        [UserAccess("Iedereen", "")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("UserInfo");
            return RedirectToAction("LoginNew", "Authentication", new {extra = "uitgelogd" });
        }
        [UserAccess("LoggedIn", "")]
        [HttpPost]
        public IActionResult ChangeLeSetting(ApplicationUser model)
        {
            if (model.currentPassword == null || model.ConfirmPassword == null || model.newPassword == null)
            {
                if(model.currentPassword == null && model.ConfirmPassword == null && model.newPassword == null)
                {
                    ChangeVal(model);
                    TempData["Status"] = "Uw gegevens zijn aangepast.";
                    return RedirectToAction("ChangeSettings");
                }
                TempData["Error"] = "Een van de waarden zijn niet ingevuld.";
                return RedirectToAction("ChangeSettings");
            }
            else
            {
                if (model.newPassword == model.ConfirmPassword)
                {
                    ChangePasswordFunc changePasswordFunc = new ChangePasswordFunc();
                    bool success = changePasswordFunc.ChangePass(model.currentPassword, model.newPassword, GetUserData.UserIDAuth(HttpContext.Session.GetString("UserInfo")));
                    if (!success)
                    {
                        TempData["Error"] = "Uw wachtwoord is niet juist.";
                        return RedirectToAction("ChangeSettings");
                    }
                    TempData["Status"] = "Uw wachtwoord is succesvol aangepast!";
                    ChangeVal(model);
                    return RedirectToAction("ChangeSettings");
                }
                else
                {
                    TempData["Error"] = "Nieuwe wachtwoorden komen niet overeen.";
                    return RedirectToAction("ChangeSettings");
                }
            }
        }
        [UserAccess("LoggedIn", "")]
        private void ChangeVal(ApplicationUser model)
        {
            string userID = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `AuthCode` = '{HttpContext.Session.GetString("UserInfo")}'")[0];
            string[] queries = { $"UPDATE `Werknemers` SET `Voornaam`='{model.naam}',`Tussenvoegsel`='{model.tussenvoegsel}',`Achternaam`='{model.achternaam}',`Email`='{model.eMail.ToLower()}',`Telefoonnummer`='{model.phoneNumber}',`Straatnaam`='{model.straatnaam}',`Huisnummer`='{model.huisNummer}',`Postcode`='{model.postcode}',`Woonplaats`='{model.woonplaats}' WHERE `UserId` = '{userID}'", $"UPDATE `Settings` SET `ReceiveMail`='{(Convert.ToBoolean(model.emailsetting)?1:0)}',`ReceiveSMS`='{(Convert.ToBoolean(model.smssetting) ? 1 : 0)}' WHERE `UserId` = '{userID}'", $"UPDATE `Login` SET `Username` = '{model.eMail}' WHERE `UserId` = '{userID}'" };
            SQLConnection.ExecuteNonSearchQueryArray(queries);
        }

        public IActionResult ChangePassword()
        {
            return View();
        } 

        [HttpPost]
        public IActionResult ChangePas(string email)
        {
            ViewData["conf"] = "good";
            ViewData["email"] = email;
            return View("ChangePassword");
        }
    }
}