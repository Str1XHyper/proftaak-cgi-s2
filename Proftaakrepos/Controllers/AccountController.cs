using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proftaakrepos.Models;
using System;

namespace Proftaakrepos.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult ChangeSettings()
        {
            ViewData["UserInfo"] = HttpContext.Session.GetString("UserInfo");
            string[] data = SQLConnection.ExecuteSearchQuery($"Select * from `Werknemers` where authCode = '{HttpContext.Session.GetString("UserInfo")}'").ToArray();
            ViewData["userInformation"] = data;
            data = SQLConnection.ExecuteSearchQuery($"SELECT * FROM `Settings` WHERE UserId = '{data[0]}'").ToArray();
            ViewData["Settings"] = data;
            return View();
        }

        public IActionResult LogOut()
        {
            
            HttpContext.Session.Remove("UserInfo");
            return RedirectToAction("Login", "Authentication");
        }

        [HttpPost]
        public IActionResult ChangeLeSetting(ApplicationUser model)
        {
            if (model.currentPassword == null || model.ConfirmPassword == null || model.newPassword == null)
            {
                if(model.currentPassword == null && model.ConfirmPassword == null && model.newPassword == null)
                {
                    ChangeVal(model);
                    return RedirectToAction("ChangeSettings");
                }
                ViewBag["Error"] = "Een van de waarden zijn niet ingevuld.";
                return RedirectToAction("ChangeSettings");
            }
            else
            {
                if (model.newPassword == model.ConfirmPassword)
                {
                    ChangePasswordFunc changePasswordFunc = new ChangePasswordFunc();
                    bool success = changePasswordFunc.ChangePass(model.currentPassword, model.newPassword, model.ConfirmPassword);
                    if (!success)
                    {
                        ViewBag["Error"] = "Wachtwoord komt niet overeen.";
                        return RedirectToAction("ChangeSettings");
                    }
                    ViewBag["Success"] = "gelukt!";
                    ChangeVal(model);
                    //func
                    return RedirectToAction("ChangeSettings");
                }
                else
                {
                    ViewBag["Error"] = "Wachtwoorden komen niet overeen.";
                    return RedirectToAction("ChangeSettings");
                }
            }
        }

        private void ChangeVal(ApplicationUser model)
        {
            string userID = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `Email` = '{model.eMail.ToLower()}'")[0];
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Werknemers` SET `Voornaam`='{model.naam}',`Tussenvoegsel`='{model.tussenvoegsel}',`Achternaam`='{model.achternaam}',`Email`='{model.eMail.ToLower()}',`Telefoonnummer`='{model.phoneNumber}',`Straatnaam`='{model.straatnaam}',`Huisnummer`='{model.huisNummer}',`Postcode`='{model.postcode}',`Woonplaats`='{model.woonplaats}' WHERE `UserId` = '{userID}'");
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `ReceiveMail`='{(Convert.ToBoolean(model.emailsetting)?1:0)}',`ReceiveSMS`='{(Convert.ToBoolean(model.smssetting) ? 1 : 0)}' WHERE `UserId` = '{userID}'");
        }
    }
}