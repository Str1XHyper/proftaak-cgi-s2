using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Proftaakrepos.Models;
using System;
using System.Threading.Tasks;

namespace Proftaakrepos.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult LoginUser()
        {
            return View();
        }
        public IActionResult AddEmployee()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LoginUser(LoginModel model)
        {
            int i = 0;
            string connStr = "server=185.182.56.248;user=bartvur381_NGF;database=bartvur381_NGF;password=Bartbart!9";
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = $"SELECT `USERNAME`, AES_DECRYPT(PASSWORD,'CGIKey')  FROM `INLOG` WHERE USERNAME='{model.Username.ToLower()}'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    i++;
                    if (rdr.GetString(1) == model.Password)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Wrong e-mail or password");
                        return View(model);
                    }
                }
                rdr.Close();
                if(i == 0)
                {
                    ModelState.AddModelError("", "Wrong e-mail or password");
                }else if(i != 1)
                {
                    ModelState.AddModelError("", "Multiple emails found, please contact a system administrator.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();

            return View(model);
        }

    }
}