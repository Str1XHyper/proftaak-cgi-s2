using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proftaakrepos.Models
{
    public class AddStatusUpdateModel
    {
        public int IncidentID { get; set; }
        public int StatusIdIncident { get; set; }
        public string StatusOmschrijving { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
