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


namespace Proftaakrepos.Controllers
{
    public class AuthenticationController : Controller
    {
        private LoginClass loginClass = new LoginClass();
        private CreateLoginCookie createLoginCookie = new CreateLoginCookie();
        private SQLConnection sQLConnection = new SQLConnection();
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            string response =  loginClass.LoginUserFunction(model.Username, model.Password).ToString();

            switch (response)
            {
                case "redirectHome":
                    string authCode = createLoginCookie.getAuthToken(model.Username);
                    HttpContext.Session.SetString("UserInfo", authCode);
                    return RedirectToAction("Index", "Home");
                case "wrongEntry":
                    ViewData["Error"] = "Wrong e-mail or password.";
                    break;
                case "multipleEntries":
                    ViewData["Error"] = "Multiple emails found, please contact a system administrator.";
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
            sQLConnection.ExecuteNonSearchQuery($"INSERT INTO `Werknemers`(`Voornaam`, `Tussenvoegsel`, `Achternaam`, `Email`, `Telefoonnummer`, `Straatnaam`, `Huisnummer`, `Postcode`, `Woonplaats`, `AuthCode`, `Rol`) VALUES ('{addEmployeeModel.naam}','{addEmployeeModel.tussenvoegsel}','{addEmployeeModel.achternaam}','{addEmployeeModel.eMail}','{addEmployeeModel.phoneNumber}','{addEmployeeModel.straatnaam}','{addEmployeeModel.huisNummer}','{addEmployeeModel.postcode}','{addEmployeeModel.woonplaats}','{authToken}','{addEmployeeModel.role}')");
            return View(addEmployeeModel);
        }

        public IActionResult AddEmployee()
        {
            return View();
        }

    }
}