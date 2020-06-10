using Logic.Verlof;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Proftaakrepos.Controllers
{

    public class VerlofController : Controller
    {
        private readonly VerlofManager verlofManager;
        public VerlofController()
        {
            verlofManager = new VerlofManager();
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
        // GET: Verlof
        [HttpGet]
        public ActionResult Aanvragen()
        {
            List<string[]> RequestData = verlofManager.VerlofverzoekenOphalen();
            ViewData["events"] = RequestData;
            return View();
        }

        public IActionResult Afwijzen(string verlofID)
        {
            verlofManager.Afwijzen(verlofID);
            return RedirectToAction("Aanvragen");
        }

        public IActionResult Goedkeuren(string verlofID)
        {
            verlofManager.Goedkeuren(verlofID);
            return RedirectToAction("Aanvragen");
        }
    }
}