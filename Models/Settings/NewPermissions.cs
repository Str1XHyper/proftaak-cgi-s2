using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Settings
{
    public class NewPermissions
    {
        public List<string> Pages { get; set; }
        public List<string> Permissions { get; set; }
        public string Rol { get; set; }
    }
}
