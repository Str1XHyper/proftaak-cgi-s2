using Models.HoursWorked;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.HoursWorked
{
    public class TimeSheetHandler
    {
        public void AddNewTimeSheet(List<ParsedTimeSheetRow> timeRows, string userID)
        {
            foreach(ParsedTimeSheetRow model in timeRows)
            {
                SQLConnection.ExecuteNonSearchQuery($"INSERT INTO Timesheet (UserID, StartDatum, EindDatum, Overuren, Type) VALUES ('{userID}', '{model.Start.ToString("yyyy-MM-dd HH:mm:ss")}', '{model.Eind.ToString("yyyy-MM-dd HH:mm:ss")}', '{model.Overuren}', '{model.Type}');");
            }
        }

        public List<string[]> GetDataFromTimeSheet(string userId, int week)
        {
            List<string[]> result = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT * FROM Timesheet WHERE UserID = {userId} && Week(StartDatum, 5) = '{week-1}'");
            return result;
        }

    }
}
