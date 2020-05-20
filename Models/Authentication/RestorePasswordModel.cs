﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class RestorePasswordModel
    {
        public string CurrentPassword{ get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string HiddenEmail { get; set; }
        public string PasswordCode { get; set; }
    }
}
