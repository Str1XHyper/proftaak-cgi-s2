using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookieManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Authentication;

namespace Proftaakrepos.Controllers
{

    public class VerlofController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TempData["Cookie"] = HttpContext.Session.GetString("UserInfo");
        }
        // GET: Verlof
        [HttpGet]
        public ActionResult Aanvragen()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RequestLeave()
        {
            return View("Aanvragen");
        }
    }
}