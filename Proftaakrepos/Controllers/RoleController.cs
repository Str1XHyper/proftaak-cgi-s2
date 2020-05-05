using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Settings;
using Proftaakrepos.Authorize;
using System;
using System.Collections.Generic;

namespace Proftaakrepos.Controllers
{
    public class RoleController : Controller
    {
        private GetPageInformation getPage;
        public RoleController()
        {
            getPage = new GetPageInformation();
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
            foreach(string perm in model.Permissions)
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