using DAL.HoursWorked;
using Models.HoursWorked;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.HoursWorked
{
    public class TimeSheetManager
    {
        private readonly TimeSheetHandler timeSheetHandler;
        public TimeSheetManager()
        {
            timeSheetHandler = new TimeSheetHandler();
        }
        public void AddNewTimeSheet(List<ParsedTimeSheetRow> timeRows, string userID)
        {
            if(userID == null || userID == string.Empty)
            {

            }
            else
            {
                timeSheetHandler.AddNewTimeSheet(timeRows, userID);
            }
        }
    }
}
