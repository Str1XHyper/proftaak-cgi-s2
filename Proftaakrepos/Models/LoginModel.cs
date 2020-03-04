using System.ComponentModel.DataAnnotations;

namespace Proftaakrepos.Models
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
