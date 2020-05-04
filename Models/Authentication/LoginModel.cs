﻿using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Username{ get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password{ get; set; }

    }
}