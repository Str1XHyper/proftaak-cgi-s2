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
    public class DeleteModel : PageModel
    {
        private readonly Proftaakrepos.Data.ProftaakreposContext _context;

        public DeleteModel(Proftaakrepos.Data.ProftaakreposContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AddEmployee = await _context.AddEmployee.FindAsync(id);

            if (AddEmployee != null)
            {
                _context.AddEmployee.Remove(AddEmployee);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
