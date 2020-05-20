using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Settings
{
    public class PasswordSettingsModel
    {
        public int Karakters { get; set; }
        public bool Nummer { get; set; }
        public bool Speciaal { get; set; }
        public bool Hoofdletter { get; set; }
        public bool KleineLetter { get; set; }
    }
}
