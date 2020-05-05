using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Settings
{
    public class PageModel
    {
        public int AmountPages { get; set; }
        public string[] Permissions { get; set; }
        public string[] PageNames { get; set; }
        public string Role { get; set; }
    }
}
