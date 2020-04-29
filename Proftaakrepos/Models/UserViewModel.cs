using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proftaakrepos.Models
{
    public class UserViewModel
    {
        public string userId { get; private set; }
        public string voornaam { get; private set; }
        public string tussenvoegsel { get; private set; }
        public string achternaam { get; private set; }
        public string rol { get; private set; }
        public UserViewModel(string userId, string voornaam, string tussenvoegsel, string achternaam, string rol)
        {
            this.userId = userId;
            this.voornaam = voornaam;
            this.tussenvoegsel = tussenvoegsel;
            this.achternaam = achternaam;
            this.rol = rol;
        }
    }
}
