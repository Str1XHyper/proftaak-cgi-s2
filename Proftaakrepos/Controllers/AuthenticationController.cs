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
        private ChangeSettings changeSettings = new ChangeSettings();
        private GetRole getRole = new GetRole();
        private AddLoginAccount addLoginAccount = new AddLoginAccount();
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
            sQLConnection.ExecuteNonSearchQuery($"INSERT INTO `Werknemers`(`Voornaam`, `Tussenvoegsel`, `Achternaam`, `Email`, `Telefoonnummer`, `Straatnaam`, `Huisnummer`, `Postcode`, `Woonplaats`, `AuthCode`, `Rol`) VALUES ('{addEmployeeModel.naam}','{addEmployeeModel.tussenvoegsel}','{addEmployeeModel.achternaam}','{addEmployeeModel.eMail.ToLower()}','{addEmployeeModel.phoneNumber}','{addEmployeeModel.straatnaam}','{addEmployeeModel.huisNummer}','{addEmployeeModel.postcode}','{addEmployeeModel.woonplaats}','{authToken}','{addEmployeeModel.role}')");
            //changeSettings.InitSettings(addEmployeeModel.eMail, addEmployeeModel.emailsetting, addEmployeeModel.smssetting);
            addLoginAccount.AddLogin(addEmployeeModel.eMail, changeSettings.InitSettings(addEmployeeModel.eMail, addEmployeeModel.emailsetting, addEmployeeModel.smssetting).ToString());
            return View(addEmployeeModel);
        }

        public IActionResult AddEmployee()
        {
            if(getRole.RoleNameAuth(HttpContext.Session.GetString("UserInfo")).ToLower() == "roostermaker")
            {
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
            string[] data = sQLConnection.ExecuteSearchQuery($"Select * from `Werknemers` where UserId = {employee}").ToArray();
            ViewData["EmployeeInfo"] = data;
            return View("Employees");
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePassword changePassword)
        {
            addLoginAccount.ChangeLoginAdmin(changePassword.email, changePassword.password);
            ViewData["msg"] = "Wachtwoord aangepast";
            return View("Employees");
        }
    }
}