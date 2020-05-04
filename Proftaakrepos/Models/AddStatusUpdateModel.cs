using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Proftaakrepos.Models
{
    public class AddStatusUpdateModel
    {
        public int IncidentID { get; set; }
        public int StatusIdIncident { get; set; }
        [Required]
        public string StatusOmschrijving { get; set; }
        [Required]
        public string Start { get; set; }
        [Required]
        public string End { get; set; }
        [Required]
        public string StatusNaam { get; set; }
    }
}
