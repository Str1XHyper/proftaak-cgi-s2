using DAL.Agenda;
using Models;
using Models.Agenda;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Logic.Planner
{
    public class AgendaManager
    {
        private readonly AgendaHandler agendahandler;
        private readonly NotificationManager notificaties;
        public AgendaManager()
        {
            if (agendahandler == null) agendahandler = new AgendaHandler();
            notificaties = new NotificationManager();
        }
        public List<EventModel> GetEvents() => agendahandler.GetEvents();
        public string[] GetVerlofCount() => agendahandler.GetVerlofCount();
        public string[] GetThemeColours() => agendahandler.GetThemeColours();
        public List<UserViewModel> GetAllUserData() => agendahandler.GetAllUserData();
        public string[] GetLoggedInUserData(string var) => agendahandler.GetLoggedInUserData(var);
        public void DeleteEvent(int eventId) => agendahandler.DeleteEvent(eventId);
        public AgendaViewModel SetAgendaViewModel(string loggedUser) => agendahandler.SetAgendaViewModel(loggedUser);
        public void UpdateEvent(DateTime start, DateTime end, string eventid, bool allday) => agendahandler.UpdateEvent(start.ToString("yyyy/MM/dd HH:mm"), end.ToString("yyyy/MM/dd HH:mm"), eventid, Convert.ToInt32(allday));
        public string[] GetUsers()
        {
            // Get names from database
            List<string[]> names = agendahandler.GetUsers();

            // Put names in correct format
            string[] parsedNames = new string[names.Count];
            for (int i = 0; i < names.Count; i++)
            {
                string completeName = "";
                foreach (string namePiece in names[i])
                {
                    completeName += namePiece + " ";
                }
                completeName.Trim();
                parsedNames[i] = completeName;
            }

            // Return name array.
            return parsedNames;
        }
        private string[] ConvertUserIDs(string input)
        {
            // Check if input is not empty
            if (input == "" || input == null)
            {
                throw new ArgumentNullException();
            }

            // Encode to prevent SQL Injection
            string[] cleanData = new string[input.Split(',').Length];
            for (int i = 0; i < input.Split(",").Length; i++)
            {
                cleanData[i] = input.Split(',')[i];
            }
            return cleanData;
        }
        private string BuildTitle(string[] row, string rol, List<string[]> names)
        {
            string title = row[2];
            if (rol.ToLower() == "roostermaker")
            {
                foreach (string[] name in names)
                {
                    if (row[1] == name[3])
                        title = name[0] + " - " + title;
                }
            }
            return title;
        }
        public List<ParseableEventModel> FetchAllEvents(string userIds, string type, string rol)
        {
            string[] uids = ConvertUserIDs(userIds);

            // Get required data
            List<string[]> names = agendahandler.GetNames();
            List<string[]> result = agendahandler.GetEventData(type, uids);
            List<string> colors = agendahandler.GetColours();

            // Set default colours
            if (colors.Count < 1)
            {
                colors = new List<string>();
                // Standby
                colors.Add("3B5A6F");
                // Incidenten
                colors.Add("353B45");
                // Pauze
                colors.Add("828A87");
                // Verlof
                colors.Add("830101");
            }

            // Put result into models
            String[] c = { "Stand-by", "Incidenten", "Pauze", "Verlof" };
            List<ParseableEventModel> returnList = new List<ParseableEventModel>();
            foreach (string[] row in result)
            {
                int index = Array.IndexOf(c, row[6]);
                string title = BuildTitle(row, rol, names);
                bool editable = rol.ToLower() == "roostermaker";
                returnList.Add(BuildEventModel(row, title, editable, colors, index));
            }

            // return parsable model for Full Calendar
            return returnList;
        }
        private ParseableEventModel BuildEventModel(string[] row, string title, bool editable, List<string> colors, int index)
        {
            return new ParseableEventModel
            {
                id = Convert.ToInt32(row[0]),
                title = title,
                start = DateTime.Parse(row[4]),
                end = DateTime.Parse(row[5]),
                backgroundColor = "#" + colors[index],
                allDay = Convert.ToBoolean(Convert.ToInt32(row[7])),
                description = row[3],
                borderColor = "#010203",
                soort = row[6],
                userId = row[1],
                editable = editable,
            };
        }
        public void CreateEvent(EventModel newmodel, string loggedUserID)
        {
            if (!string.IsNullOrEmpty(newmodel.userId))
            {
                // Cut off the comma at the end
                if (newmodel.userId.EndsWith(",")) newmodel.userId = newmodel.userId.Substring(0, newmodel.userId.Length - 1);

                // Convert userid string to string[]
                string[] uids = ConvertUserIDs(newmodel.userId);

                // Insert new event into database
                agendahandler.CreateEvent(uids, newmodel);

                // When an event has type "Verlof" it creates a new Absence request
                string eventID = agendahandler.GetLatestEventID();
                bool isVerlof = agendahandler.CreateAbsenceRequest(eventID, newmodel, loggedUserID);
                if (!isVerlof) notificaties.SendInplanning(loggedUserID, eventID);
            }
        }
    }
}
