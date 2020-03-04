using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Proftaakrepos.Models;
using Microsoft.EntityFrameworkCore;

namespace Proftaakrepos.Views.Home
{
    public class EmployeesModel : PageModel
    {
        private readonly Proftaakrepos.Data.ProftaakreposContext _context;

        public IList<AddEmployee> AddEmployee { get; set; }

        public async Task OnGetAsync()
        {
            AddEmployee = await _context.AddEmployee.ToListAsync();
        }
    }
}
