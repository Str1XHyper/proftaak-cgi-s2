using ClassLibrary.Classes;
using CookieManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Authentication;
using Models.Settings;
using Proftaakrepos.Authorize;
using System;
using System.Collections.Generic;

namespace Proftaakrepos.Controllers
{
    public class RoleController : Controller
    {
        private readonly ICookieManager _cookieManager;
        private GetPageInformation getPage;
        public RoleController(ICookieManager cookiemanager)
        {
            _cookieManager = cookiemanager;
            getPage = new GetPageInformation();
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TempData["Cookie"] = HttpContext.Session.GetString("UserInfo");
        }
        [UserAccess("", "Toegang")]
        public IActionResult Index()
        {
            ViewBag.Rollen = getPage.GetRoles();
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
            getPage.InsertNewPermissions(model.Rol, permissions, model.Pages);
            ViewBag.Rollen = getPage.GetRoles();
            return View("Index");
        }
        [UserAccess("", "Toegang")]
        [Route("/Role/GetPermissions/{rol}")]
        public IActionResult GetPermissions(string rol)
        {
            ViewBag.Pages = getPage.GetPages(rol);
            ViewBag.Rollen = getPage.GetRoles();
            return View("Index");
        }
    }
}