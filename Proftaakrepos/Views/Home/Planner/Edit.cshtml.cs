using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proftaakrepos.Data;
using Proftaakrepos.Models;

namespace Proftaakrepos
{
    public class EditModel : PageModel
    {
        private readonly Proftaakrepos.Data.ProftaakreposContext _context;

        public EditModel(Proftaakrepos.Data.ProftaakreposContext context)
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

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(AddEmployee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddEmployeeExists(AddEmployee.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AddEmployeeExists(int id)
        {
            return _context.AddEmployee.Any(e => e.ID == id);
        }
    }
}
