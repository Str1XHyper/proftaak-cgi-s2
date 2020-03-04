using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proftaakrepos.Models;
using System.Threading.Tasks;

namespace Proftaakrepos.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthenticationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Login(LoginModel lm)
        //{
        //    if (ModelState.IsValid)
        //    {

        //    }
        //    return View(lm);
        //}
        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEmployeeModel addEmployeeModel)
        {
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser { UserName = addEmployeeModel.GebruikersNaam };
                var result = await _userManager.CreateAsync(user, addEmployeeModel.Wachtwoord);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(addEmployeeModel);

        }

    }
}