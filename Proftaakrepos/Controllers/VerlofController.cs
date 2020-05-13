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
        private readonly ICookieManager _cookieManager;
        public VerlofController(ICookieManager cookiemanager)
        {
            _cookieManager = cookiemanager;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TempData["CookieMonster"] = _cookieManager.Get<CookieModel>("BIER.User");
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