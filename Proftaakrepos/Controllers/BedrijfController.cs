﻿using CookieManager;
using Logic.Authentication.Access;
using Logic.Employees;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Agenda;
using Models.Authentication;
using Models.Settings;
using Proftaakrepos.Authorize;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Logic.Bedrijf;

namespace Proftaakrepos.Controllers
{
    public class BedrijfController : Controller
    {
        private readonly BedrijfManager bedrijfManager;
        public BedrijfController()
        {
            bedrijfManager = new BedrijfManager();
        }
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
        [UserAccess("", "Bedrijfsinstellingen")]
        public IActionResult AgendaSettings()
        {
            string[] colours = bedrijfManager.GetColors();
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
            AccessManager accessManager = new AccessManager();
            ViewBag.Password = accessManager.GetSettings();
            return View();
        }
        [UserAccess("", "Bedrijfsinstellingen")]
        [HttpPost]
        public IActionResult AgendaSettings(SettingsPageModel model)
        {
            AgendaSettings settings = model.model2;
            bedrijfManager.DeleteColours();
            bedrijfManager.AddColours(settings.standbyKleur, settings.incidentKleur, settings.pauzeKleur, settings.verlofKleur);
            return RedirectToAction("AgendaSettings");
        }

        public IActionResult DeleteColours()
        {
            bedrijfManager.DeleteColours();
            return RedirectToAction("AgendaSettings");
        }

        [UserAccess("", "Bedrijfsinstellingen")]
        [HttpPost]
        public IActionResult PasswordSettings(SettingsPageModel model)
        {
            EmployeeInfoManager employeesManager = new EmployeeInfoManager();
            List<string> values = new List<string>();
            values.Add(Convert.ToInt32(model.model1.Nummer).ToString());
            values.Add(Convert.ToInt32(model.model1.Speciaal).ToString());
            values.Add(Convert.ToInt32(model.model1.Hoofdletter).ToString());
            values.Add(Convert.ToInt32(model.model1.KleineLetter).ToString());
            values.Add(model.model1.Karakters.ToString());
            employeesManager.SetPasswordSettings(values);
            return RedirectToAction("AgendaSettings");
        }
    }
}