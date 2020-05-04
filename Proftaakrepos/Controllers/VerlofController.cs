using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Proftaakrepos.Controllers
{
    public class VerlofController : Controller
    {
        // GET: Verlof
        public ActionResult Index()
        {
            return View();
        }
    }
}