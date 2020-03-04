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
    public class IndexModel : PageModel
    {
        private readonly Proftaakrepos.Data.ProftaakreposContext _context;

        public IndexModel(Proftaakrepos.Data.ProftaakreposContext context)
        {
            _context = context;
        }

        public IList<AddEmployee> AddEmployee { get;set; }

        public async Task OnGetAsync()
        {
            AddEmployee = await _context.AddEmployee.ToListAsync();
        }
    }
}
