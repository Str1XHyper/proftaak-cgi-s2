using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CookieManager;
using Models.Language;

namespace Proftaakrepos.Controllers
{
    public class LanguageController : Controller
    {
        private readonly ICookieManager _cookieManager;
        private readonly ICookie _cookie;

        public LanguageController(ICookieManager cookieManager, ICookie cookie)
        {
            this._cookieManager = cookieManager;
            this._cookie = cookie;

        }

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
            LanguageCookieModel lcm = new LanguageCookieModel();
            lcm.Language = language;
            _cookieManager.Set("BIER.User.Culture", lcm, 10 * 365 * 24 * 60 * 60);
            return RedirectToAction(action, controller);
        }
    }
}