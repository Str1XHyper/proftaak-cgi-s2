using DAL.Agenda;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace DAL.Employees
{
    public class EmployeesHandler
    {
        private AgendaHandler agendaHandler;
        public EmployeesHandler()
        {
            agendaHandler = new AgendaHandler();
        }
        public List<string[]> GetStandByEmployees()
        {
            List<string[]> roosterData = agendaHandler.GetAllRoosterData();
            List<string[]> userData = GetAllEmployees();
            List<string[]> users = new List<string[]>();

            foreach (string[] roosterEvent in roosterData)
            {
                DateTime start = DateTime.Parse(roosterEvent[1]);
                DateTime end = DateTime.Parse(roosterEvent[2]);
                if (DateTime.Compare(start, DateTime.Now) < 0 && DateTime.Compare(end, DateTime.Now) > 0)
                {
                    foreach (string[] userInfo in userData)
                    {
                        bool userInList = false;
                        if (userInfo[0] == roosterEvent[0])
                        {
                            if (users.Count > 0)
                            {
                                foreach (string[] user in users)
                                {
                                    if (user[0] == userInfo[0])
                                    {
                                        userInList = true;
                                        break;
                                    }
                                }
                            }
                            if (!userInList)
                            {
                                users.Add(userInfo);
                            }
                        }
                    }
                }
            }
            return users;
        }

        public List<string[]> GetAllEmployees()
        {
            return SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT UserId,Email,Voornaam FROM Werknemers");
        }

        public int GetAmountOfEmployees() => SQLConnection.ExecuteSearchQuery("SELECT `UserId` FROM `Werknemers`").ToArray().Length;
        public string[] GetEmployeeNames() => SQLConnection.ExecuteSearchQuery("SELECT `Voornaam`, `Tussenvoegsel`, `Achternaam` FROM `Werknemers`").ToArray();
        public List<string[]> GetNamesAndUserIDs() => SQLConnection.ExecuteSearchQueryWithArrayReturn("SELECT `Voornaam`, `Tussenvoegsel`, `Achternaam`, `UserId` FROM `Werknemers`");
        public string[] EmployeeInfo(string naam) => SQLConnection.ExecuteSearchQuery($"SELECT * FROM `Werknemers` WHERE `VOORNAAM` = {naam}").ToArray();
        public List<string[]> EmployeesInfoWithEmailSetting(string userID)
        {
            List<string[]> response = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT Werknemers.Email, Werknemers.Voornaam FROM Werknemers INNER JOIN Settings ON Werknemers.UserId=Settings.UserID where Settings.ReceiveMail=1 and not Werknemers.UserID='{userID}'");
            if (response.Count > 0) return response;
            else return new List<string[]>();
        }
    }
}
