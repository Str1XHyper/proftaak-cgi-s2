using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Classes;
using Microsoft.AspNetCore.Mvc;
using Models.Agenda;
using Proftaakrepos.Authorize;

namespace Proftaakrepos.Controllers
{
    public class BedrijfController : Controller
    {
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
            return View();
        }
        [UserAccess("", "Bedrijfsinstellingen")]
        [HttpPost]
        public IActionResult AgendaSettings(AgendaSettings settings)
        {
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM ColorScheme");
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO ColorScheme (StandBy,Incidenten,Pauze,Verlof) VALUES ('{settings.standbyKleur}','{settings.incidentKleur}','{settings.pauzeKleur}','{settings.verlofKleur}')");
            return RedirectToAction("Agenda");
        }

        public IActionResult DeleteColours()
        {
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM ColorScheme");
            return RedirectToAction("Agenda");
        }
    }
}