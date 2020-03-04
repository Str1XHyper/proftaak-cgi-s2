using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proftaakrepos.Data;
using Proftaakrepos.Models;

namespace Proftaakrepos
{
    public class DetailsModel : PageModel
    {
        private readonly Proftaakrepos.Data.ProftaakreposContext _context;

        public DetailsModel(Proftaakrepos.Data.ProftaakreposContext context)
        {
            _context = context;
        }

        public AddEmployee AddEmployee { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AddEmployee = await _context.AddEmployee.FirstOrDefaultAsync(m => m.ID == id);

            if (AddEmployee == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
