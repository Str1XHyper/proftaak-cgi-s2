using DAL.Agenda;
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
            incidentenHandler = new IncidentenHelper();
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
        public string[] GetUsersIncidentIDs(string userID)
        {
            List<string> ids = new List<string>();
            List<string[]> incidentids = incidentenHandler.GetAllIncidentIDs();
            List<EventModel> events = new AgendaHandler().GetStandByEvents(new string[] { userID});
            List<string> usersincidentids = new List<string>();
            foreach(string[] row in incidentids)
            {
                foreach (EventModel Event in events)
                {
                    if (Convert.ToDateTime(row[1]) >= Event.startDate && Convert.ToDateTime(row[1]) <= Event.endDate) //Start datum zit tussen start en eind van stand-by
                        AddID(row[0], row[3], ids, usersincidentids);
                    else if (Convert.ToDateTime(row[2]) >= Event.startDate && Convert.ToDateTime(row[2]) <= Event.endDate) //Eind zit tussen start en eind van stand-by
                        AddID(row[0], row[3], ids, usersincidentids);
                    else if (Convert.ToDateTime(row[1]) >= Event.startDate && Convert.ToDateTime(row[2]) <= Event.endDate) //Incident tussen start en eind van stand-by
                        AddID(row[0], row[3], ids, usersincidentids);
                    else if (Convert.ToDateTime(row[1]) <= Event.startDate && Convert.ToDateTime(row[2]) >= Event.endDate) //Incident buiten start en eind van stand-by
                        AddID(row[0], row[3], ids ,usersincidentids);
                }
            }
            return usersincidentids.ToArray();
        }

        private void AddID(string id, string name, List<string> ids, List<string> incidenten)
        {
            bool duplicate = false;
            foreach(string iID in ids)
            {
                if (iID == id) duplicate = true;
            }
            if (!duplicate)
            {
                ids.Add(id);
                incidenten.Add(name);
            }
        }
    }
}
