using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Models;
using Proftaakrepos.Authorize;
using CookieManager;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Authentication;
using System.Globalization;
using System.Threading;
using Models.Incidenten;
using Logic;
using Logic.Incidenten;

namespace Proftaakrepos.Controllers
{
    public class IncidentsController : Controller
    {
        IncidentenManager incidentenManager = new IncidentenManager();
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

        [UserAccess("","Incidenten")]
        [HttpGet]
        public IActionResult Index(string status, int? statusId, IncidentMailModel model)
        {
            if(status != null && statusId != null)
            {
                if(status == "1")
                {
                    incidentenManager.StartIncident(Convert.ToInt32(statusId));
                }
                else if (status == "2")
                {
                    incidentenManager.FinishIncident(Convert.ToInt32(statusId), model);
                }
            }
            ViewBag.Incidents = incidentenManager.GetIncidents();
            ViewBag.IncidentUpdateCount = incidentenManager.GetIncidentUpdateCount();
            return View();
        }

        [UserAccess("", "Incidenten")]
        [HttpGet]
        public IActionResult StatusUpdate(int incidentId, bool delete, int? updateId)
        {
            if (delete)
            {
                if(updateId != null)
                {
                    incidentenManager.DeleteStatusUpdate(Convert.ToInt32(updateId));
                }
            }
            var statusUpdates = incidentenManager.GetIncidentUpdates(incidentId);
            ViewBag.StatusUpdates = statusUpdates;
            ViewBag.IncidentId = incidentId;
            return View();
        }

        [UserAccess("", "Incidenten")]
        public IActionResult AddUpdate(int incidentId)
        {
            var statusUpdates = incidentenManager.GetIncidentUpdates(incidentId);
            ViewBag.StatusUpdates = statusUpdates;
            ViewBag.IncidentId = incidentId;
            return View();
        }

        [UserAccess("", "Incidenten")]
        [HttpPost]
        public IActionResult AddUpdate(int incidentId, AddStatusUpdateModel model)
        {
            incidentenManager.AddStatusUpdate(model);
            var statusUpdates = incidentenManager.GetIncidentUpdates(incidentId);
            ViewBag.StatusUpdates = statusUpdates;
            ViewBag.IncidentId = incidentId;
            return RedirectToAction("StatusUpdate", "Incidents", new { incidentId = incidentId });
        }

        [UserAccess("", "Incidenten")]
        public IActionResult EditUpdate(int incidentId, int statusIdIncident, string statusOmschrijving, string start, string end, string statusNaam)
        {
            AddStatusUpdateModel model = new AddStatusUpdateModel
            {
                IncidentID = incidentId,
                StatusIdIncident = statusIdIncident,
                StatusOmschrijving = statusOmschrijving,
                Start = start,
                End = end,
                StatusNaam = statusNaam
            };
            return View(model);
        }

        [UserAccess("", "Incidenten")]
        [HttpPost]
        public IActionResult EditUpdate(AddStatusUpdateModel model)
        {
            incidentenManager.EditStatusUpdate(model);
            return RedirectToAction("StatusUpdate", "Incidents", new { incidentId = model.IncidentID });
        }

        [UserAccess("", "Incidenten")]
        public IActionResult VoegIncidentToe()
        {
            return View();
        }

        [UserAccess("", "Incidenten")]
        [HttpPost]
        public ActionResult VoegIncidentToe(AddIncidentModel model)
        {
            NotificationManager notifications = new NotificationManager();
            incidentenManager.AddIncident(model);
            bool succeeded = notifications.NotifyStandBy(model);
            if (succeeded)
            {
                Console.WriteLine("Mail has been sent");
            } else
            {
                Console.WriteLine("No user found that is stand by");
            }
            return RedirectToAction("Index");
        }
    }
}