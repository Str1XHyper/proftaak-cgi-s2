using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Proftaakrepos.Controllers
{
    public class LanguageController : Controller
    {
        public static string language = "en";
        public IActionResult Index(string language, string currentPage)
        {
            if (!string.IsNullOrEmpty(language))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            }
            string controller = currentPage.Split('/')[1];
            string action = currentPage.Split('/')[2];
            HttpContext.Session.SetString("Culture", language);
            return RedirectToAction(action, controller);
        }
    }
}