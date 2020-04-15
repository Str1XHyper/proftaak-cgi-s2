using Microsoft.AspNetCore.Identity;

namespace Proftaakrepos.Models
{
    public class ApplicationUser
    {
        public string naam { get; set; }
        public string tussenvoegsel { get; set; }
        public string achternaam { get; set; }
        public string eMail { get; set; }
        public int phoneNumber { get; set; }
        public string straatnaam { get; set; }
        public int huisNummer { get; set; }
        public string postcode { get; set; }
        public string woonplaats { get; set; }
        public string role { get; set; }

        public string emailsetting { get; set; }
        public string smssetting { get; set; }
        public string currentPassword{ get; set; }
        public string newPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
