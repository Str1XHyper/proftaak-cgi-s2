using DAL.HoursWorked;
using DAL.Incidenten;
using Models;
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
        private readonly IncidentenHelper incidentenHandler;
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
        public List<string> GetUsersIncidentIDs(string UserID, List<EventModel> eventlist)
        {
            List<string[]> incidentids = incidentenHandler.GetAllIncidentIDs();
            List<string> usersincidentids = new List<string>();
            foreach(string[] row in incidentids)
            {
                foreach(EventModel model in eventlist)
                {
                    if(model.startDate >= Convert.ToDateTime(row[1]) && model.endDate <= Convert.ToDateTime(row[1]))
                        usersincidentids.Add(row[0]);
                    else if(model.startDate >= Convert.ToDateTime(row[2]) && model.endDate <= Convert.ToDateTime(row[2]))
                        usersincidentids.Add(row[0]);
                    else if(model.startDate >= Convert.ToDateTime(row[1]) && model.endDate <= Convert.ToDateTime(row[2]))
                        usersincidentids.Add(row[0]);
                }
            }
            return usersincidentids;
        }
    }
}
