using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Models;

namespace Proftaakrepos.Controllers
{
    public class IncidentsController : Controller
    {
        [HttpGet]
        public IActionResult Index(string? status, int? statusId)
        {
            if(status != null && statusId != null)
            {
                SQLConnection.ExecuteNonSearchQuery($"UPDATE `Incidenten` SET `Afgehandeld`= '{status}' WHERE `IncidentID` = '{statusId}'");
                if(status == "1")
                {
                    SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `IncidentUpdates`(`IncidentID`, `StatusIDIncident`, `StatusOmschrijving`, `Start`, `End`) VALUES ('{statusId}','0','Begonnen aan het incident','{DateTime.Now.ToString("yyyy/MM/dd HH:mm")}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm")}')");
                } else if (status == "2")
                {
                    int i = Convert.ToInt32(SQLConnection.ExecuteSearchQuery($"SELECT COUNT(`StatusIDIncident`) FROM  `IncidentUpdates` WHERE `IncidentID` = '{statusId}'")[0]);
                    SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `IncidentUpdates`(`IncidentID`, `StatusIDIncident`, `StatusOmschrijving`, `Start`, `End`) VALUES ('{statusId}','{i}','Incident is afgehandled','{DateTime.Now.ToString("yyyy/MM/dd HH:mm")}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm")}')");
                }
            }
            var incidents = SQLConnection.ExecuteSearchQueryWithArrayReturn("SELECT * FROM `Incidenten` WHERE `Afgehandeld` = '0' OR `Afgehandeld` = '1'");
            ViewBag.Incidents = incidents;
            string _authCode = HttpContext.Session.GetString("UserInfo");
            if (CheckIfAllowed.IsAllowed(_authCode, "Incident")) return View();
            else return RedirectToAction("NoAccessIndex", "Home");
        }

        [HttpGet]
        public IActionResult StatusUpdate(int? incidentId, bool delete, int? updateId)
        {
            if (delete)
            {
                if(updateId != null)
                {
                    SQLConnection.ExecuteNonSearchQuery($"DELETE FROM `IncidentUpdates` WHERE `StatusId` = '{updateId}'");
                }
            }
            var statusUpdates = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT * FROM `IncidentUpdates` WHERE `IncidentID` = '{incidentId}' ORDER BY `StatusIdIncident` ASC");
            ViewBag.StatusUpdates = statusUpdates;
            ViewBag.IncidentId = incidentId;
            return View();
        }

        public IActionResult AddUpdate(int incidentId)
        {
            var statusUpdates = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT * FROM `IncidentUpdates` WHERE `IncidentID` = '{incidentId}'");
            ViewBag.StatusUpdates = statusUpdates;
            ViewBag.IncidentId = incidentId;
            return View();
        }

        [HttpPost]
        public IActionResult AddUpdate(int incidentId, AddStatusUpdateModel addStatusUpdateModel)
        {
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `IncidentUpdates`(`IncidentID`, `StatusIDIncident`, `StatusOmschrijving`, `Start`, `End`) VALUES ('{addStatusUpdateModel.IncidentID}','{addStatusUpdateModel.StatusIdIncident}','{addStatusUpdateModel.StatusOmschrijving}','{DateTime.Parse(addStatusUpdateModel.Start).ToString("yyyy/MM/dd HH:mm")}','{DateTime.Parse(addStatusUpdateModel.End).ToString("yyyy/MM/dd HH:mm")}')");
            var statusUpdates = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT * FROM `IncidentUpdates` WHERE `IncidentID` = '{incidentId}'");
            ViewBag.StatusUpdates = statusUpdates;
            ViewBag.IncidentId = incidentId;
            return RedirectToAction("StatusUpdate", "Incidents", new { incidentId = incidentId });
        }

        public IActionResult EditUpdate(int incidentId, int statusIdIncident, string statusOmschrijving, string start, string end)
        {
            AddStatusUpdateModel model = new AddStatusUpdateModel
            {
                IncidentID = incidentId,
                StatusIdIncident = statusIdIncident,
                StatusOmschrijving = statusOmschrijving,
                Start = start,
                End = end
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditUpdate(AddStatusUpdateModel model)
        {
           
            SQLConnection.ExecuteNonSearchQuery($" UPDATE `IncidentUpdates` SET `StatusOmschrijving`='{model.StatusOmschrijving}',`Start`='{DateTime.Parse(model.Start).ToString("yyyy/MM/dd HH:mm")}',`End`='{DateTime.Parse(model.End).ToString("yyyy/MM/dd HH:mm")}' WHERE `IncidentId` = '{model.IncidentID}' AND `StatusIdIncident` = '{model.StatusIdIncident}'");
            return RedirectToAction("StatusUpdate", "Incidents", new { incidentId = model.IncidentID });
        }

        public IActionResult VoegIncidentToe()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VoegIncidentToe(AddIncidentModel model)
        {
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `Incidenten`(`Omschrijving`) VALUES ('{model.IncidentNaam}')");
            return RedirectToAction("Index");
        }
    }
}