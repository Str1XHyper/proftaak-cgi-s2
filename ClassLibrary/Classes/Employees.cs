using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Classes
{
    public class Employees
    {
        private string[] countResults;
        public int Count()
        {
            countResults = SQLConnection.ExecuteSearchQuery("SELECT `UserId` FROM `Werknemers`").ToArray();
            return countResults.Length;
        }

        public string[] Names()
        {
            return SQLConnection.ExecuteSearchQuery("SELECT `Voornaam`, `Tussenvoegsel`, `Achternaam` FROM `Werknemers`").ToArray();
        }

        public List<string[]> NamesTest()
        {
            return SQLConnection.ExecuteSearchQueryWithArrayReturn("SELECT `Voornaam`, `Tussenvoegsel`, `Achternaam`, `UserId` FROM `Werknemers`");
        }

        public string[] EmployeeInfo(string name)
        {
            return countResults;
        }
    }
}
