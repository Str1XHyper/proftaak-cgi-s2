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
using System.Collections.Generic;

namespace Proftaakrepos.Controllers
{
    public class AuthenticationController : Controller
    {
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
                    string authCode = CreateLoginCookie.getAuthToken(model.Username);
                    HttpContext.Session.SetString("UserInfo", authCode);
                    return RedirectToAction("Index", "Home");
                case "wrongEntry":
                    ViewData["Error"] = "Verkeerde e-mail of wachtwoord combinatie.";
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
        [HttpPost]
        public IActionResult AddEmployee(AddEmployee addEmployeeModel)
        {
            string authToken = GenerateAuthToken.GetUniqueKey(10);
            string newEmail = addEmployeeModel.eMail.ToLower();
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `Werknemers`(`Voornaam`, `Tussenvoegsel`, `Achternaam`, `Email`, `Telefoonnummer`, `Straatnaam`, `Huisnummer`, `Postcode`, `Woonplaats`, `AuthCode`, `Rol`) VALUES ('{addEmployeeModel.naam}','{addEmployeeModel.tussenvoegsel}','{addEmployeeModel.achternaam}','{newEmail}','{addEmployeeModel.phoneNumber}','{addEmployeeModel.straatnaam}','{addEmployeeModel.huisNummer}','{addEmployeeModel.postcode}','{addEmployeeModel.woonplaats}','{authToken}','{addEmployeeModel.role}')");
            AddLoginAccount.AddLogin(addEmployeeModel.eMail, ChangeSettings.InitSettings(addEmployeeModel.eMail, addEmployeeModel.emailsetting, addEmployeeModel.smssetting).ToString());
            ViewData["result"] = "Werknemer " + addEmployeeModel.naam + " toegevoegd!";
            return View(addEmployeeModel);
        }

        [HttpPost]
        public IActionResult UpdateEmployee(AddEmployee addEmployeeModel)
        {
            string userID = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `Email` = '{addEmployeeModel.eMail.ToLower()}'")[0];
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Werknemers` SET `Voornaam`='{addEmployeeModel.naam}',`Tussenvoegsel`='{addEmployeeModel.tussenvoegsel}',`Achternaam`='{addEmployeeModel.achternaam}',`Email`='{addEmployeeModel.eMail.ToLower()}',`Telefoonnummer`='{addEmployeeModel.phoneNumber}',`Straatnaam`='{addEmployeeModel.straatnaam}',`Huisnummer`='{addEmployeeModel.huisNummer}',`Postcode`='{addEmployeeModel.postcode}',`Woonplaats`='{addEmployeeModel.woonplaats}',`Rol`='{addEmployeeModel.role}' WHERE `UserId` = {userID}");
            return View("Employees");
        }

        public IActionResult AddEmployee()
        {
            if(GetUserData.RoleNameAuth(HttpContext.Session.GetString("UserInfo")).ToLower() == "roostermaker")
            {
                List<string> typeOfRoles = SQLConnection.ExecuteSearchQuery($"SELECT `Naam` from `Rollen`");
                ViewData["roles"] = typeOfRoles.ToArray();
                return View();
            }
            return RedirectToAction("NoAccessIndex", "Home");
        }

        public IActionResult Employees()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetEmployeeInfo(string employee)
        {
            List<string> totalRoles = new List<string>();
            string[] data = SQLConnection.ExecuteSearchQuery($"Select * from `Werknemers` where UserId = {employee}").ToArray();
            ViewData["EmployeeInfo"] = data;
            List<string> typeOfRoles = SQLConnection.ExecuteSearchQuery($"SELECT `Naam` from `Rollen`");
            totalRoles.Add(data[11]);
            for(int i = 0; i < typeOfRoles.Count; i++)
            {
                if (data[11].ToLower() != typeOfRoles[i].ToLower())
                {
                    totalRoles.Add(typeOfRoles[i]);
                }
            }
            ViewData["roles"] = totalRoles.ToArray();
            return View("Employees");
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePassword changePassword)
        {
            AddLoginAccount.ChangeLoginAdmin(changePassword.email, changePassword.password);
            ViewData["msg"] = "Wachtwoord aangepast";
            return View("Employees");
        }
    }
}