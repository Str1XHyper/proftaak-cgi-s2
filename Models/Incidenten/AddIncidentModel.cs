using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class AddIncidentModel
    {
        [Required]
        public string IncidentNaam { get; set; }

        [Required]
        public string IncidentOmschrijving { get; set; }
    }
}
