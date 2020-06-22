using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.HoursWorked
{
    public class OverviewTableHandler
    {
        public List<string[]> GetTimeSheetEventsByWeek(int week) => SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT * FROM Timesheet WHERE Week(StartDatum, 1) = '{week}' OR Week(EindDatum, 1) = '{week}'");
    }
}
