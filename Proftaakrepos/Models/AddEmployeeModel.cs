using System.ComponentModel.DataAnnotations;

namespace Proftaakrepos.Models
{
    public class AddEmployeeModel
    {
        [Required]
        public int ID { get; set; }

        [Required, EmailAddress, MaxLength(256), Display(Name = "Email adres")]
        public string GebruikersNaam { get; set; }

        [Required, MinLength(5), MaxLength(50), DataType(DataType.Password), Display(Name ="Wachtwoord")]
        public string Wachtwoord { get; set; }

        [Required, MinLength(5), MaxLength(50), DataType(DataType.Password), Display(Name = "Bevestig wachtwoord")]
        [Compare("Wachtwoord", ErrorMessage = "Het ingevulde wachtwoord komt niet overeen")]
        public string BevestigWachtwoord { get; set; }
    }
}
