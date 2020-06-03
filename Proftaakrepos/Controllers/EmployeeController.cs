using ClassLibrary.Classes;
using CookieManager;
using Logic.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models;
using Models.Authentication;
using Proftaakrepos.Authorize;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Proftaakrepos.Controllers
{
    public class EmployeeController : Controller
    {
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
        #region Views

        public IActionResult Employees()
        {
            return RedirectToAction("GetEmployeeInfo");
        }

        [UserAccess("", "Werknemers")]
        public IActionResult EmployeesView()
        {
            return View("Employees");
        }

        [UserAccess("", "Voeg werknemer toe")]
        public IActionResult AddEmployee()
        {
                List<string> typeOfRoles = SQLConnection.ExecuteSearchQuery($"SELECT `Naam` from `Rollen`");
                ViewData["roles"] = typeOfRoles.ToArray();
                return View();
        }
        #endregion
        #region Logic

        [UserAccess("", "Voeg werknemer toe")]
        [HttpPost]
        public IActionResult AddEmployee(AddEmployee addEmployeeModel)
        {
            LoginManager loginManager = new LoginManager();
            string authToken = GenerateAuthToken.GetUniqueKey(10);
            string newEmail = addEmployeeModel.eMail.ToLower();
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `Werknemers`(`Voornaam`, `Tussenvoegsel`, `Achternaam`, `Email`, `Telefoonnummer`, `Straatnaam`, `Huisnummer`, `Postcode`, `Woonplaats`, `AuthCode`, `Rol`) VALUES ('{addEmployeeModel.naam}','{addEmployeeModel.tussenvoegsel}','{addEmployeeModel.achternaam}','{newEmail}','{addEmployeeModel.phoneNumber}','{addEmployeeModel.straatnaam}','{addEmployeeModel.huisNummer}','{addEmployeeModel.postcode}','{addEmployeeModel.woonplaats}','{authToken}','{addEmployeeModel.role}')");
            loginManager.CreateNewLogin(addEmployeeModel.naam, ChangeSettings.InitSettings(addEmployeeModel.eMail, addEmployeeModel.emailsetting, addEmployeeModel.smssetting, addEmployeeModel.whatsappSetting).ToString(), addEmployeeModel.eMail);
            ViewData["result"] = "Werknemer " + addEmployeeModel.naam + " toegevoegd!";
            return View(addEmployeeModel);
        }

        [UserAccess("", "Werknemers")]
        [HttpPost]
        public IActionResult UpdateEmployee(AddEmployee addEmployeeModel, string oldEmail)
        {
            if(addEmployeeModel.naam != null)
            {
                string userID = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `Email` = '{oldEmail.ToLower()}'")[0];
                SQLConnection.ExecuteNonSearchQuery($"UPDATE `Werknemers` SET `Voornaam`='{addEmployeeModel.naam}',`Tussenvoegsel`='{addEmployeeModel.tussenvoegsel}',`Achternaam`='{addEmployeeModel.achternaam}',`Email`='{addEmployeeModel.eMail.ToLower()}',`Telefoonnummer`='{addEmployeeModel.phoneNumber}',`Straatnaam`='{addEmployeeModel.straatnaam}',`Huisnummer`='{addEmployeeModel.huisNummer}',`Postcode`='{addEmployeeModel.postcode}',`Woonplaats`='{addEmployeeModel.woonplaats}',`Rol`='{addEmployeeModel.role}' WHERE `UserId` = {userID}");
                SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `ReceiveMail`='{(Convert.ToBoolean(addEmployeeModel.emailsetting) ? 1 : 0)}',`ReceiveSMS`='{(Convert.ToBoolean(addEmployeeModel.smssetting) ? 1 : 0)}',`ReceiveWhatsApp`='{(Convert.ToBoolean(addEmployeeModel.whatsappSetting) ? 1 : 0)}' WHERE `UserId` = {userID}");
                SQLConnection.ExecuteNonSearchQuery($"UPDATE `Login` SET `Username` = '{addEmployeeModel.eMail}' WHERE `UserId` = '{userID}'");
                NotificationSettings settings = new NotificationSettings();
                settings.PasSettingsAan(addEmployeeModel.TypeOfAge, addEmployeeModel.ValueOfNoti, userID);
                TempData["msg"] = "Werknemer " + addEmployeeModel.naam + " is aangepast.";
            }
            return RedirectToAction("GetEmployeeInfo");
        }

        [UserAccess("", "Werknemers")]
        public IActionResult GetEmployeeInfo(string employee)
        {
            if (employee == null) employee = "1";
            List<string> totalRoles = new List<string>();
            string[] data = SQLConnection.ExecuteSearchQuery($"Select * from `Werknemers` where UserId = {employee}").ToArray();
            ViewData["EmployeeInfo"] = data;
            List<string> typeOfRoles = SQLConnection.ExecuteSearchQuery($"SELECT `Naam` from `Rollen`");
            totalRoles.Add(data[11]);
            for (int i = 0; i < typeOfRoles.Count; i++)
            {
                if (data[11].ToLower() != typeOfRoles[i].ToLower())
                {
                    totalRoles.Add(typeOfRoles[i]);
                }
            }
            ViewData["roles"] = totalRoles.ToArray();
            return View("Employees");
        }

        #endregion
    }
}