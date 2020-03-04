﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proftaakrepos.Models
{
    public class AddEmployee
    {
        public int ID{ get; set; }
        public string name{ get; set; }
        public string tussenvoegsel { get; set; }
        public string achternaam{ get; set; }
        public string eMail { get; set; }
        public int phoneNumber{ get; set; }
        public string straatName{ get; set; }
        public int huisNummer { get; set; }
        public string postcode{ get; set; }
        public string woonplaats{ get; set; }
    }
}