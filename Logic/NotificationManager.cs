using ClassLibrary.Classes;
using DAL.Employees;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

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
                    var email = user[0];
                    SendMail.SendEmployeeIncident("bartdgp@outlook.com", "Bart Vermeulen", model.IncidentOmschrijving, model.IncidentNaam);
                    SendMail.SendKlantIncident("bartdgp@outlook.com", "Bart Vermeulen", model.IncidentOmschrijving, model.IncidentNaam);
                }
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
