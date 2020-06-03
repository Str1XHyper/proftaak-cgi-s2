using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Classes;
using CookieManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models;
using Models.Agenda;
using Models.Authentication;
using Ubiety.Dns.Core.Records.NotUsed;
using Proftaakrepos.Authorize;
using Logic.Planner;

namespace Proftaakrepos.Controllers
{
    public class PlannerController : Controller
    {
        private static string userId;
        private static string rol;
        private AgendaManager agendamanager = new AgendaManager();
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

        [UserAccess("", "Rooster")]
        public List<ParseableEventModel> FetchAllEvents(string userIds, string type) => agendamanager.FetchAllEvents(userIds, type, rol);

        [UserAccess("", "Rooster wijzigen")]
        public string[] GetUsers() => agendamanager.GetUsers();

        //[UserAccess("", "Rooster wijzigen")]
        [HttpPost]
        public void CreateEvent(EventModel newmodel) => agendamanager.CreateEvent(newmodel, userId);

        [UserAccess("", "Rooster")]
        public IActionResult Schedule()
        {
            string var = HttpContext.Session.GetString("UserInfo");
            string[] loggedUserData = agendamanager.GetLoggedInUserData(var);
            rol = loggedUserData[0];
            userId = loggedUserData[1];
            HttpContext.Session.SetString("Image", loggedUserData[2]);
            if (rol.ToLower() == "roostermaker")
                ViewData["verlof"] = agendamanager.GetVerlofCount();
            ViewData["rol"] = rol;
            ViewData["userId"] = userId;
            ViewData["language"] = "nl";
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Culture")))
                ViewData["language"] = HttpContext.Session.GetString("Culture");
            return View();
        }

        [UserAccess("", "Rooster wijzigen")]
        public void UpdateEvent(DateTime start, DateTime end, string eventid, bool allday) => agendamanager.UpdateEvent(start, end, eventid, allday);

        public void DeleteEvent(int EventId) => agendamanager.DeleteEvent(EventId);
    }
}