using DAL.Bedrijf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Bedrijf
{
    public class BedrijfManager
    {
        private readonly BedrijfHandler bedrijfHandler;
        public BedrijfManager()
        {
            bedrijfHandler = new BedrijfHandler();
        }
        public void DeleteColours() => bedrijfHandler.DeleteColours();
        public void AddColours(string standbyKleur, string incidentKleur, string pauzeKleur, string verlofKleur) => bedrijfHandler.AddColours(standbyKleur, incidentKleur, pauzeKleur, verlofKleur);
        public string[] GetColors() => bedrijfHandler.GetColors();
    }
}
