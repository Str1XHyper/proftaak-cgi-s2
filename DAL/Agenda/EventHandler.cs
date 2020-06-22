using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Agenda
{
    public class EventHandler
    {
        public List<string[]> GetEventsByWeek(int week) => SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT * FROM Rooster WHERE Week(Start, 1) = '{week}' OR Week(End, 1) = '{week}'");
    }
}
