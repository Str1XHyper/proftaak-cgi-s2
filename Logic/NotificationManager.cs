using DAL.Employees;
using Models;
using Models.Incidenten;
using System.Collections.Generic;

namespace Logic
{
    public class NotificationManager
    {
        private readonly EmployeesHandler employeesHandler;
        public NotificationManager()
        {
            employeesHandler = new EmployeesHandler();
        }
        public bool NotifyStandBy(AddIncidentModel model)
        {
            List<string[]> users = employeesHandler.GetStandByEmployees();
            if (users.Count > 0)
            {
                foreach (string[] user in users)
                {
                    string email = user[0];
                    //SendMail.SendEmployeeIncident("bartdgp@outlook.com", "Bart Vermeulen", model.IncidentOmschrijving, model.IncidentNaam);
                    //SendMail.SendKlantIncident("bartdgp@outlook.com", "Bart Vermeulen", model.IncidentOmschrijving, model.IncidentNaam);
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public void NotifySolved(IncidentMailModel model)
        {
            //SendMail.SendEmployeeIncidentSolved("BartDGP@outlook.com", "Bart Vermeulen", model.Beschrijving, model.Naam);
            //SendMail.SendKlantIncidentSolved("BartDGP@outlook.com", "Bart Vermeulen", model.Beschrijving, model.Naam);
        }
    }
}
