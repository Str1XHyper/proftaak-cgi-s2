using CookieManager;
using Logic.Authentication.Access;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Settings;
using Proftaakrepos.Authorize;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Proftaakrepos.Controllers
{
    public class RoleController : Controller
    {
        private readonly ICookieManager _cookieManager;
        private AccessManager accessManager;
        public RoleController(ICookieManager cookiemanager)
        {
            _cookieManager = cookiemanager;
            accessManager = new AccessManager();
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
        [UserAccess("", "Toegang")]
        public IActionResult Index()
        {
            ViewBag.Rollen = accessManager.GetRoles();
            return View();
        }
        [UserAccess("", "Toegang")]
        public IActionResult ChangePermissions(NewPermissions model)
        {
            List<int> permissions = new List<int>();
            for (int i = 0; i < model.Pages.Count; i++)
            {
                if (model.Pages[i] == "Toegang" && model.Rol.ToLower() == "roostermaker")
                {
                    model.Permissions[i] = "true";
                }
            }
            foreach (string perm in model.Permissions)
            {
                permissions.Add(Convert.ToInt32(Convert.ToBoolean(perm)));
            }
            accessManager.InsertNewPermissions(model.Rol, permissions, model.Pages);
            ViewBag.Rollen = accessManager.GetRoles();
            return View("Index");
        }
        [UserAccess("", "Toegang")]
        [Route("/Role/GetPermissions/{rol}")]
        public IActionResult GetPermissions(string rol)
        {
            ViewBag.Pages = accessManager.GetPages(rol);
            ViewBag.Rollen = accessManager.GetRoles();
            return View("Index");
        }
    }
}