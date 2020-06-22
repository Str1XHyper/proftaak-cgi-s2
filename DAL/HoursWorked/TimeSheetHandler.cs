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
                SQLConnection.ExecuteNonSearchQuery($"INSERT INTO Timesheet (UserID, StartDatum, EindDatum, OverUren, Type) VALUES ({userID}, '{model.Start.ToString("yyyy-MM-dd hh:mm:ss")}', '{model.Eind.ToString("yyyy-MM-dd hh:mm:ss")}', '{model.Overuren}', '{model.Type}');");
            }
        }

    }
}
