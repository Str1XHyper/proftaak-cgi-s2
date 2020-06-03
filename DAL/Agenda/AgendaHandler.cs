using ClassLibrary.Classes;
using Models;
using Models.Agenda;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Agenda
{
    public class AgendaHandler
    {
        public List<string[]> GetAllRoosterData() => SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT UserId,Start,End FROM Rooster");
        public string[] GetThemeColours() => SQLConnection.ExecuteSearchQuery($"select * from ColorScheme").ToArray();
        public string[] GetLoggedInUserData(string var) => SQLConnection.ExecuteSearchQuery($"Select Rol,UserId,ProfielFoto From Werknemers Where AuthCode = '{var}'").ToArray();
        public string[] GetVerlofCount() => SQLConnection.ExecuteSearchQuery($"Select Count(*) From Verlofaanvragen where Geaccepteerd = '-1'").ToArray();
        public List<string[]> GetUsers() => SQLConnection.ExecuteSearchQueryWithArrayReturn("SELECT `UserId`, `Voornaam`, `Tussenvoegsel`, `Achternaam` FROM `Werknemers` ORDER BY `UserId` ASC");
        public List<string[]> GetNames() => SQLConnection.ExecuteSearchQueryWithArrayReturn("SELECT `Voornaam`, `Tussenvoegsel`, `Achternaam`, `UserId` FROM `Werknemers`");
        public List<string> GetColours() => SQLConnection.ExecuteSearchQuery("SELECT * FROM `ColorScheme`");
        public string GetLatestEventID() => SQLConnection.ExecuteSearchQuery($"SELECT MAX(EventId) FROM Rooster")[0];
        public void UpdateEvent(string start, string end, string eventid, int allday) => SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set Start = '{start}',End = '{end}',IsFullDay = '{allday}' Where EventId = '{eventid}'");
        public void DeleteEvent(int eventID)
        {
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM Rooster WHERE EventId = {eventID}");
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM Verlofaanvragen WHERE EventId = {eventID}");
        }
        public List<EventModel> GetEvents()
        {
            List<EventModel> eventList = new List<EventModel>();
            string[] roosterData = SQLConnection.ExecuteSearchQuery($"Select Rooster.*, Werknemers.Voornaam From Rooster INNER JOIN Werknemers ON Werknemers.UserId = Rooster.UserId").ToArray();
            for (int i = 0; i < roosterData.Length; i += 10)
            {
                EventModel model = new EventModel()
                {
                    eventId = Convert.ToInt32(roosterData[i]),
                    userId = roosterData[i + 1],
                    title = roosterData[i + 2],
                    description = roosterData[i + 3],
                    startDate = Convert.ToDateTime(roosterData[i + 4]),
                    endDate = Convert.ToDateTime(roosterData[i + 5]),
                    themeColor = roosterData[i + 6],
                    isFullDay = Convert.ToInt32(roosterData[i + 7]),
                    voornaam = roosterData[i + 9],
                };
                eventList.Add(model);
            }
            return eventList;
        }
        public List<UserViewModel> GetAllUserData()
        {
            List<UserViewModel> userList = new List<UserViewModel>();
            string[] userData = SQLConnection.ExecuteSearchQuery($"Select UserId, Voornaam, Tussenvoegsel, Achternaam, Rol From Werknemers").ToArray();
            for (int i = 0; i < userData.Length; i += 5)
            {
                userList.Add(new UserViewModel(userData[i], userData[i + 1], userData[i + 2], userData[i + 3], userData[i + 4]));
            }
            return userList;
        }
        public AgendaViewModel SetAgendaViewModel(string loggedUser)
        {
            AgendaViewModel viewdata = new AgendaViewModel(loggedUser);
            viewdata.eventList = GetEvents();
            viewdata.userList = GetAllUserData();
            return viewdata;
        }
        public List<string[]> GetEventData(string type, string[] uids)
        {
            string sqlQuery = $"Select * from `Rooster` WHERE `UserId` = '{uids[0]}'";
            if (type != "all")
            {
                sqlQuery += $"AND `ThemeColor` = '{type}'";
            }
            if (uids.Length > 1)
            {
                for (int i = 1; i < uids.Length; i++)
                {
                    if (!string.IsNullOrEmpty(uids[i]))
                    {
                        sqlQuery += $"OR `UserId` = '{uids[i]}' ";
                        if (type != "all")
                        {
                            sqlQuery += $"AND `ThemeColor` = '{type}'";
                        }
                    }
                }
            }
            return SQLConnection.ExecuteSearchQueryWithArrayReturn(sqlQuery);
        }
        public void CreateEvent(string[] uids, EventModel newmodel)
        {
            string sqlquery = $"INSERT INTO Rooster(UserId, Subject, Description, Start, End, ThemeColor, IsFullDay, IsPending) VALUES ";
            for (int i = 0; i < uids.Length; i++)
            {
                if (i > 0 && !string.IsNullOrEmpty(uids[i]))
                    sqlquery += ",";
                if (!string.IsNullOrEmpty(uids[i]))
                    sqlquery += $"('{uids[i]}', '{newmodel.title}', '{newmodel.description}', '{newmodel.startDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{newmodel.endDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{newmodel.themeColor}', '{(newmodel.isFullDay)}', '{(newmodel.isPending ? 1 : 0)}')";
            }
            SQLConnection.ExecuteNonSearchQuery(sqlquery);
        }
        public void CreateAbsenceRequest(string eventID, EventModel newmodel, string loggedUserID)
        {
            if (newmodel.themeColor.ToLower() == "verlof")
            {
                SQLConnection.ExecuteNonSearchQuery($"INSERT INTO Verlofaanvragen (UserID, EventID) VALUES ('{loggedUserID}', '{eventID}')");
            }
            else
            {
                NotificationSettings settings = new NotificationSettings();
                settings.SendRoosterNotifcation(newmodel.userId, eventID);
            }
        }
    }
}
