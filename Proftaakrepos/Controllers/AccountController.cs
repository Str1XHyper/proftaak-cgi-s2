using ClassLibrary.Classes;
using CookieManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models;
using Models.Authentication;
using Proftaakrepos.Authorize;
using System;
using System.Threading.Tasks;

namespace Proftaakrepos.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICookieManager _cookieManager;
        public AccountController(ICookieManager cookieManager)
        {
            _cookieManager = cookieManager;
        }
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
            _cookieManager.Remove("BIER.User");
            return RedirectToAction("LoginNew", "Authentication", new {extra = "uitgelogd" });
        }
        [UserAccess("LoggedIn", "")]
        [HttpPost]
        public IActionResult ChangeLeSetting(ApplicationUser model)
        {
            TempData["CookieMonster"] = _cookieManager.Get<CookieModel>("BIER.User");
            if (model.currentPassword == null || model.ConfirmPassword == null || model.newPassword == null)
            {
                if(model.currentPassword == null && model.ConfirmPassword == null && model.newPassword == null)
                {
                    ChangeVal(model);
                    TempData["Status"] = "Uw gegevens zijn aangepast.";
                    return RedirectToAction("ChangeSettings");
                }
                TempData["Error"] = "Een van de waarden is niet ingevuld.";
                return RedirectToAction("ChangeSettings");
            }
            else
            {
                if (model.newPassword == model.ConfirmPassword)
                {
                    ChangePasswordFunc changePasswordFunc = new ChangePasswordFunc();
                    GetUserData userData = new GetUserData();
                    bool success = changePasswordFunc.ChangePass(model.currentPassword, model.newPassword, userData.UserIDAuth(HttpContext.Session.GetString("UserInfo")));
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
            TempData["CookieMonster"] = _cookieManager.Get<CookieModel>("BIER.User");
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

        public IActionResult PasswordChange(string authcode)
        {
            ViewData["auth"] = authcode;
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(RestorePasswordModel model)
        {
            ChangePasswordFunc changePassword = new ChangePasswordFunc();
            if(changePassword.ChangePassAuthCode(model.ConfirmPassword, model.NewPassword, model.HiddenEmail) == true)
            {
                TempData["success"] = "Wachtwoord is aangepast, u kunt nu inloggen.";
                return RedirectToAction("LoginNew", "Authentication");
            }
            else
            {
                TempData["success"] = "Wachtwoorden waren niet gelijk, vraag een nieuwe resetlink aan.";
                return View();
            }
        }
    }
}