using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Proftaakrepos.Controllers
{
    public class RoleController : Controller
    {
        public IActionResult Index()
        {
            string _authCode = HttpContext.Session.GetString("UserInfo");
            if (CheckIfAllowed.IsAllowed(_authCode, "RoleIndex")) return View();
            else return RedirectToAction("NoAccessIndex", "Home");
        }
    }
}