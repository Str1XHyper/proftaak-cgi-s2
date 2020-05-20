using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookieManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Authentication;
using ClassLibrary.Classes;

namespace Proftaakrepos.Controllers
{

    public class VerlofController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TempData["Cookie"] = HttpContext.Session.GetString("UserInfo");
        }
        // GET: Verlof
        [HttpGet]
        public ActionResult Aanvragen()
        {
            int? UserID = HttpContext.Session.GetInt32("UserInfo.ID");
            List<string[]> requests = SQLConnection.ExecuteSearchQueryWithArrayReturn($"Select * FROM `Verlofaanvragen` WHERE `Geaccepteerd`='-1'");
            List<string[]> names = SQLConnection.ExecuteSearchQueryWithArrayReturn($"Select `UserId`, `Voornaam`, `Tussenvoegsel`, `Achternaam` FROM `Werknemers`");
            List<string[]> RequestData = new List<string[]>();
            foreach (string[] request in requests)
            {
                foreach (string[] name in names)
                {
                    if (request[1] == name[0])
                    {
                        string[] temp = new string[request.Length + 3];
                        if (name[2] != string.Empty)
                        {
                            temp[0] = name[1] + " " + name[2] + " " + name[3];
                        } else
                        {
                            temp[0] = name[1] + " " + name[3];
                        }
                        for(int i = 1; i <= request.Length; i++)
                        {
                            temp[i] = request[i - 1];
                        }
                        RequestData.Add(temp);
                    }
                }
            }
            ViewData["events"] = RequestData;
            return View();
        }

        public IActionResult Afwijzen(string verlofID)
        {
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Verlofaanvragen` SET `Geaccepteerd`='0' WHERE `VerlofID` = '{verlofID}'");
            return RedirectToAction("Aanvragen");
        }

        public IActionResult Goedkeuren(string verlofID)
        {
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Verlofaanvragen` SET `Geaccepteerd` = '1' WHERE `VerlofID` = '{verlofID}'");
            return RedirectToAction("Aanvragen");
        }
    }
}