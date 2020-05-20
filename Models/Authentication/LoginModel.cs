using System.ComponentModel.DataAnnotations;

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

        [Required]
        public string IP { get; set; }

        public bool Remember { get; set; }

    }
}
