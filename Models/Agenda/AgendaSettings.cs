using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Agenda
{
    public class AgendaSettings
    {
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
