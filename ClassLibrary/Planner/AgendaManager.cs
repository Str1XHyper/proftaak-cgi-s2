using ClassLibrary.Classes;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Planner
{
    public class AgendaManager
    {
        public string[] GetThemeColours()
        {
            return SQLConnection.ExecuteSearchQuery($"select * from ColorScheme").ToArray();
        }
        public string[] GetLoggedInUserData(string var)
        {
            return SQLConnection.ExecuteSearchQuery($"Select Rol,UserId From Werknemers Where AuthCode = '{var}'").ToArray();
        }
        public List<EventModel> GetEvents()
        {
            List<EventModel> eventList = new List<EventModel>();
            string[] roosterData = SQLConnection.ExecuteSearchQuery($"Select Rooster.*, Werknemers.Voornaam From Rooster INNER JOIN Werknemers ON Werknemers.UserId = Rooster.UserId").ToArray();
            for (int i = 0; i < roosterData.Length; i += 10)
            {
                EventModel model = new EventModel();
                model.eventId = Convert.ToInt32(roosterData[i]);
                model.userId = roosterData[i + 1];
                model.title = roosterData[i + 2];
                model.description = roosterData[i + 3];
                model.startDate = Convert.ToDateTime(roosterData[i + 4]);
                model.endDate = Convert.ToDateTime(roosterData[i + 5]);
                model.themeColor = roosterData[i + 6];
                model.isFullDay = Convert.ToInt32(roosterData[i + 7]);
                model.voornaam = roosterData[i + 9];
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
                UserViewModel usermodel = new UserViewModel(userData[i], userData[i + 1], userData[i + 2], userData[i + 3], userData[i + 4]);
                userList.Add(usermodel);
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
        public string[] GetVerlofCount()
        {
                return SQLConnection.ExecuteSearchQuery($"Select Count(*) From Verlofaanvragen where Geaccepteerd = '-1'").ToArray();
        }
    }
}
