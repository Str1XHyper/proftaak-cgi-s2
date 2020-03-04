using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Proftaakrepos.Views.Home
{
    public class Index1Model : PageModel
    {
        public void OnPostTest()
        {
            Console.WriteLine("Click!");
            RedirectToAction("Privacy");
        }
    }
}
