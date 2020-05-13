using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Classes;
using CookieManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Agenda;
using Models.Authentication;
using Models.Settings;
using Proftaakrepos.Authorize;

namespace Proftaakrepos.Controllers
{
    public class BedrijfController : Controller
    {
        private readonly ICookieManager _cookieManager;
        public BedrijfController(ICookieManager cookiemanager)
        {
            _cookieManager = cookiemanager;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TempData["CookieMonster"] = _cookieManager.Get<CookieModel>("BIER.User");
        }
        [UserAccess("", "Bedrijfsinstellingen")]
        public IActionResult AgendaSettings()
        {
            string[] colours = SQLConnection.ExecuteSearchQuery($"SELECT * FROM ColorScheme").ToArray();
            if (colours.Length == 0)
            {
                //Default colours to display if none are selected by company
                colours = new string[4];
                colours[0] = "3b5a6f";
                colours[1] = "828a87";
                colours[2] = "353b45";
                colours[3] = "830101";
            }
            ViewData["colours"] = colours;
            GetPageInformation getInformation = new GetPageInformation();
            ViewBag.Password = getInformation.GetSettings();
            return View();
        }
        [UserAccess("", "Bedrijfsinstellingen")]
        [HttpPost]
        public IActionResult AgendaSettings(SettingsPageModel model)
        {
            AgendaSettings settings = model.model2;
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM ColorScheme");
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO ColorScheme (StandBy,Incidenten,Pauze,Verlof) VALUES ('{settings.standbyKleur}','{settings.incidentKleur}','{settings.pauzeKleur}','{settings.verlofKleur}')");
            return RedirectToAction("AgendaSettings");
        }

        public IActionResult DeleteColours()
        {
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM ColorScheme");
            return RedirectToAction("Agenda");
        }

        [UserAccess("", "Bedrijfsinstellingen")]
        [HttpPost]
        public IActionResult PasswordSettings(SettingsPageModel model)
        {
            ChangeSettings changeSettings = new ChangeSettings();
            List<string> values = new List<string>();
            values.Add(Convert.ToInt32(model.model1.Nummer).ToString());
            values.Add(Convert.ToInt32(model.model1.Speciaal).ToString());
            values.Add(Convert.ToInt32(model.model1.Hoofdletter).ToString());
            values.Add(Convert.ToInt32(model.model1.KleineLetter).ToString());
            values.Add(model.model1.Karakters.ToString());
            changeSettings.SetPasswordSettings(values);
            return RedirectToAction("AgendaSettings");
        }
    }
}