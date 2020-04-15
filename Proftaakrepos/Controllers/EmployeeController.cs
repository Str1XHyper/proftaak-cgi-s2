using ClassLibrary.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Proftaakrepos.Models;
using System.Collections.Generic;

namespace Proftaakrepos.Controllers
{
    public class EmployeeController : Controller
    {
        #region Views
        public IActionResult Employees()
        {
            return View();
        }

        public IActionResult AddEmployee()
        {
            if (GetUserData.RoleNameAuth(HttpContext.Session.GetString("UserInfo")).ToLower() == "roostermaker")
            {
                List<string> typeOfRoles = SQLConnection.ExecuteSearchQuery($"SELECT `Naam` from `Rollen`");
                ViewData["roles"] = typeOfRoles.ToArray();
                return View();
            }
            return RedirectToAction("NoAccessIndex", "Home");
        }
        #endregion
        #region Logic
        [HttpPost]
        public IActionResult AddEmployee(AddEmployee addEmployeeModel)
        {
            string authToken = GenerateAuthToken.GetUniqueKey(10);
            string newEmail = addEmployeeModel.eMail.ToLower();
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `Werknemers`(`Voornaam`, `Tussenvoegsel`, `Achternaam`, `Email`, `Telefoonnummer`, `Straatnaam`, `Huisnummer`, `Postcode`, `Woonplaats`, `AuthCode`, `Rol`) VALUES ('{addEmployeeModel.naam}','{addEmployeeModel.tussenvoegsel}','{addEmployeeModel.achternaam}','{newEmail}','{addEmployeeModel.phoneNumber}','{addEmployeeModel.straatnaam}','{addEmployeeModel.huisNummer}','{addEmployeeModel.postcode}','{addEmployeeModel.woonplaats}','{authToken}','{addEmployeeModel.role}')");
            AddLoginAccount.AddLogin(addEmployeeModel.naam, ChangeSettings.InitSettings(addEmployeeModel.eMail, addEmployeeModel.emailsetting, addEmployeeModel.smssetting).ToString(), addEmployeeModel.eMail);
            ViewData["result"] = "Werknemer " + addEmployeeModel.naam + " toegevoegd!";
            return View(addEmployeeModel);
        }

        [HttpPost]
        public IActionResult UpdateEmployee(AddEmployee addEmployeeModel)
        {
            string userID = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `Email` = '{addEmployeeModel.eMail.ToLower()}'")[0];
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Werknemers` SET `Voornaam`='{addEmployeeModel.naam}',`Tussenvoegsel`='{addEmployeeModel.tussenvoegsel}',`Achternaam`='{addEmployeeModel.achternaam}',`Email`='{addEmployeeModel.eMail.ToLower()}',`Telefoonnummer`='{addEmployeeModel.phoneNumber}',`Straatnaam`='{addEmployeeModel.straatnaam}',`Huisnummer`='{addEmployeeModel.huisNummer}',`Postcode`='{addEmployeeModel.postcode}',`Woonplaats`='{addEmployeeModel.woonplaats}',`Rol`='{addEmployeeModel.role}' WHERE `UserId` = {userID}");
            return View(addEmployeeModel);
        }

        [HttpPost]
        public IActionResult GetEmployeeInfo(string employee)
        {
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