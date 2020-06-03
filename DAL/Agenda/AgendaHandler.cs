using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Agenda
{
    public class AgendaHandler
    {
        public List<string[]> GetAllRoosterData()
        {
            return SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT UserId,Start,End FROM Rooster");
        }
    }
}
