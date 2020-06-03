using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Bedrijf
{
    public class BedrijfHandler
    {
        public void DeleteColours() => SQLConnection.ExecuteNonSearchQuery($"DELETE FROM ColorScheme");
        public void AddColours(string standbyKleur, string incidentKleur, string pauzeKleur, string verlofKleur) => SQLConnection.ExecuteNonSearchQuery($"INSERT INTO ColorScheme (StandBy,Incidenten,Pauze,Verlof) VALUES ('{standbyKleur}','{incidentKleur}','{pauzeKleur}','{verlofKleur}')");
        public string[] GetColors() => SQLConnection.ExecuteSearchQuery($"SELECT * FROM ColorScheme").ToArray();
    }
}
