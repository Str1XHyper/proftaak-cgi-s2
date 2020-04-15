using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Proftaakrepos.Models;

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
            }
            var incidents = SQLConnection.ExecuteSearchQueryWithArrayReturn("SELECT * FROM `Incidenten` WHERE `Afgehandeld` = '0' OR `Afgehandeld` = '1'");
            ViewBag.Incidents = incidents;
            return View();
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
        public IActionResult EditUpdate(AddStatusUpdateModel addStatusUpdateModel)
        {
           
            SQLConnection.ExecuteNonSearchQuery($" UPDATE `IncidentUpdates` SET `StatusOmschrijving`='{addStatusUpdateModel.StatusOmschrijving}',`Start`='{DateTime.Parse(addStatusUpdateModel.Start).ToString("yyyy/MM/dd HH:mm")}',`End`='{DateTime.Parse(addStatusUpdateModel.End).ToString("yyyy/MM/dd HH:mm")}' WHERE `IncidentId` = '{addStatusUpdateModel.IncidentID}' AND `StatusIdIncident` = '{addStatusUpdateModel.StatusIdIncident}'");
            return RedirectToAction("StatusUpdate", "Incidents", new { incidentId = addStatusUpdateModel.IncidentID });
        }
    }
}