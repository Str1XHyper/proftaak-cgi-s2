using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class AddStatusUpdateModel
    {
        public int IncidentID { get; set; }
        public int StatusIdIncident { get; set; }
        public string StatusOmschrijving { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
