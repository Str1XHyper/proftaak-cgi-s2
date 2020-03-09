using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Proftaakrepos.Models;

namespace Proftaakrepos.Data
{
    public class ProftaakreposContext : DbContext
    {
        public ProftaakreposContext (DbContextOptions<ProftaakreposContext> options)
            : base(options)
        {
        }

        public DbSet<Proftaakrepos.Models.AddEmployee> AddEmployee { get; set; }
    }
}
