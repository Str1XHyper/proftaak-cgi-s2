using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proftaakrepos.Models
{
    public class AgendaSettings
    {
        //public string standbyKleur = "3b5a6f";
        //public string pauzeKleur = "828a87";
        //public string incidentKleur = "353b45";
        //public string verlofKleur = "830101";
        public string standbyKleur { get; set; }
        public string pauzeKleur { get; set; }
        public string incidentKleur { get; set; }
        public string verlofKleur { get; set; }
        public AgendaSettings(string standbyKleur, string pauzeKleur, string incidentKleur, string verlofKleur)
        {
            this.standbyKleur = standbyKleur;
            this.pauzeKleur = pauzeKleur;
            this.incidentKleur = incidentKleur;
            this.verlofKleur = verlofKleur;
        }
        public AgendaSettings()
        {

        }
    }
}
