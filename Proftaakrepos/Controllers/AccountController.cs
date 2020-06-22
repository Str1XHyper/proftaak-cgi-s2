using CookieManager;
using Logic.Authentication.Login;
using Logic.Employees;
using Logic.Reset;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models;
using Models.Authentication;
using Proftaakrepos.Authorize;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DAL;
using Logic;
using System.Threading;
using System.Globalization;

namespace Proftaakrepos.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICookieManager _cookieManager;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public AccountController(ICookieManager cookieManager, IWebHostEnvironment _hostingEnvironment)
        {
            _cookieManager = cookieManager;
            this._hostingEnvironment = _hostingEnvironment;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TempData["Cookie"] = HttpContext.Session.GetString("UserInfo");
            string language = HttpContext.Session.GetString("Culture");
            if (!string.IsNullOrEmpty(language))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            }
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
            HttpContext.Session.Remove("UserInfo");
            return RedirectToAction("LoginNew", "Authentication", new {extra = Proftaakrepos.Resources.lang.Uitgelogd });
        }
        [UserAccess("LoggedIn", "")]
        [HttpPost]
        public async Task<IActionResult> ChangeLeSetting(ApplicationUser model, IFormFile image)
        {
            #region Image shit
            string filename = "";
            if (image != null)
            {
                string prevImg = HttpContext.Session.GetString("Image");
                if (!string.IsNullOrEmpty(prevImg))
                {
                    ImageManager.DeleteImage(prevImg);
                }
                filename = ImageManager.GetImageName(image.ContentType);
                await ImageManager.SaveImage(image, Path.Combine(_hostingEnvironment.WebRootPath, "uploadedimages"), filename);
            }
            #endregion
            if (model.currentPassword == null || model.ConfirmPassword == null || model.newPassword == null)
            {
                if(model.currentPassword == null && model.ConfirmPassword == null && model.newPassword == null)
                {
                    ChangeVal(model, filename);
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
                    LoginManager loginManager = new LoginManager();
                    EmployeeInfoManager employeeInfo = new EmployeeInfoManager();
                    bool success = loginManager.ChangePassword(model.currentPassword, model.newPassword, employeeInfo.IDFromAuth(HttpContext.Session.GetString("UserInfo")).ToString());
                    if (!success)
                    {
                        TempData["Error"] = "Uw wachtwoord is niet juist.";
                        return RedirectToAction("ChangeSettings");
                    }
                    TempData["Status"] = "Uw wachtwoord is succesvol aangepast!";
                    ChangeVal(model, filename);
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
        private void ChangeVal(ApplicationUser model, string imagepath)
        {
            if(imagepath != "")
                HttpContext.Session.SetString("Image", imagepath);
            else
            {
                imagepath= HttpContext.Session.GetString("Image");
            }
            string userID = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `AuthCode` = '{HttpContext.Session.GetString("UserInfo")}'")[0];
            string[] queries = { $"UPDATE `Werknemers` SET `Voornaam`='{model.naam}',`Tussenvoegsel`='{model.tussenvoegsel}',`Achternaam`='{model.achternaam}',`Email`='{model.eMail.ToLower()}',`Telefoonnummer`='{model.phoneNumber}',`Straatnaam`='{model.straatnaam}',`Huisnummer`='{model.huisNummer}',`Postcode`='{model.postcode}',`Woonplaats`='{model.woonplaats}', ProfielFoto='{imagepath}' WHERE `UserId` = '{userID}'", $"UPDATE `Settings` SET `ReceiveMail`='{(Convert.ToBoolean(model.emailsetting)?1:0)}',`ReceiveSMS`='{(Convert.ToBoolean(model.smssetting) ? 1 : 0)}',`ReceiveWhatsApp`='{(Convert.ToBoolean(model.whatsappSetting) ? 1 : 0)}' WHERE `UserId` = '{userID}'", $"UPDATE `Login` SET `Username` = '{model.eMail}' WHERE `UserId` = '{userID}'", $"UPDATE `HeadsUpSetting` SET `UserID`='{userID}', `Amount`='{model.ValueOfNoti}', `Type`='{model.TypeOfAge}'" };
            SQLConnection.ExecuteNonSearchQueryArray(queries);
            NotificationManager notificaties = new NotificationManager();
            notificaties.PasInstellingenAan(model.TypeOfAge, model.ValueOfNoti, userID);
        }

        public IActionResult ChangePassword()
        {
            return View();
        } 

        [HttpPost]
        public IActionResult ChangePas(string email)
        {
            ViewData["conf"] = "good";
            string[] values = Proftaakrepos.Resources.lang.Aangevraagd.Split('-');
            ViewData["msg"] = values[0] + email + values[1];
            ViewData["email"] = email;
            ResetManager reset = new ResetManager();
            reset.SendReset(email);
            return View("ChangePassword");
        }

        public IActionResult PasswordChange(string authcode, string code)
        {
            ViewData["auth"] = authcode;
            ViewData["code"] = code;
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(RestorePasswordModel model)
        {
            ResetManager resetManager = new ResetManager();
            if (resetManager.ChangeResetPassword(model.ConfirmPassword, model.NewPassword, model.HiddenEmail, model.PasswordCode))
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