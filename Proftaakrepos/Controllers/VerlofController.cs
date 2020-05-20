using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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
            string language = HttpContext.Session.GetString("Culture");
            if (!string.IsNullOrEmpty(language))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            }
        }
        // GET: Verlof
        [HttpGet]
        public ActionResult Aanvragen()
        {
            int? UserID = HttpContext.Session.GetInt32("UserInfo.ID");
            List<string[]> requests = SQLConnection.ExecuteSearchQueryWithArrayReturn($"Select * FROM `Verlofaanvragen` WHERE `Geaccepteerd`='-1'");
            List<string[]> names = SQLConnection.ExecuteSearchQueryWithArrayReturn($"Select `UserId`, `Voornaam`, `Tussenvoegsel`, `Achternaam` FROM `Werknemers`");
            List<string[]> info = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT `UserId`, `Description`, `Start`, `End`, `IsFullDay` FROM `Rooster` WHERE `ThemeColor`='Verlof'");
            List<string[]> RequestData = new List<string[]>();
            foreach (string[] request in requests)
            {
                foreach (string[] name in names)
                {
                    if (request[1] == name[0])
                    {
                        string[] temp = new string[request.Length + 4];
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
                        foreach(string[] array in info)
                        {
                            if(request[1] == array[0])
                            {
                                temp[temp.Length - 3] = array[1];
                                temp[temp.Length - 2] = array[2];
                                temp[temp.Length - 1] = array[3];
                            }
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
            List<string> response = SQLConnection.ExecuteSearchQuery($"SELECT `EventID` FROM `Verlofaanvragen` WHERE `VerlofID`='{verlofID}'");
            List<string> info = SQLConnection.ExecuteSearchQuery($"SELECT Rooster.EventId, Rooster.Subject FROM `Rooster` INNER JOIN `Verlofaanvragen` ON Rooster.EventId = Verlofaanvragen.EventID WHERE Verlofaanvragen.VerlofID = '{verlofID}'");
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Rooster` SET `Subject` = '{info[1]} - Afgewezen' WHERE `EventId` = '{info[0]}'");
            return RedirectToAction("Aanvragen");
        }

        public IActionResult Goedkeuren(string verlofID)
        {
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Verlofaanvragen` SET `Geaccepteerd` = '1' WHERE `VerlofID` = '{verlofID}'");
            List<string> info = SQLConnection.ExecuteSearchQuery($"SELECT Rooster.EventId, Rooster.Subject FROM `Rooster` INNER JOIN `Verlofaanvragen` ON Rooster.EventId = Verlofaanvragen.EventID WHERE Verlofaanvragen.VerlofID = '{verlofID}'");
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Rooster` SET `Subject` = '{info[1]} - Geaccepteerd' WHERE `EventId` = '{info[0]}'");
            return RedirectToAction("Aanvragen");
        }
    }
}