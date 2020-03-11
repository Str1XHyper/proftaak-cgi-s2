using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Classes
{
    public class Employees
    {
        SQLConnection sQLConnection = new SQLConnection();
        private string[] countResults;
        public int Count()
        {
            countResults = sQLConnection.ExecuteSearchQuery("SELECT `UserId` FROM `Werknemers`").ToArray();
            return countResults.Length;
        }

        public string[] Names()
        {
            return sQLConnection.ExecuteSearchQuery("SELECT `Voornaam`, `Tussenvoegsel`, `Achternaam` FROM `Werknemers`").ToArray();
        }

        public string[] EmployeeInfo(string name)
        {
            return countResults;
        }
    }
}
