using ClassLibrary.Classes;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Planner
{
    public class EventManager
    {
        public string[] GetSelectedUsersEvents(string userID, string SendUserId)
        {
            string[] userIdArray = new string[1];
            if (SendUserId == null || SendUserId == "0")
            {
                userIdArray[0] = userID;
            }
            else
            {
                SendUserId = SendUserId.Substring(0, SendUserId.Length - 1);
                userIdArray = SendUserId.Split(",");
            }
            return userIdArray;
        }
        public List<string[]> GetEventData(string[] userIdArray)
        {
            if (userIdArray[0] == "-1" && userIdArray.Length == 1)
            {
                return SQLConnection.ExecuteSearchQueryWithArrayReturn($"select Rooster.*, Werknemers.Voornaam from Rooster INNER JOIN Werknemers ON Rooster.UserId = Werknemers.UserId");
            }
            else if (userIdArray[0] == "0" && userIdArray.Length == 1)
            {
                return SQLConnection.ExecuteSearchQueryWithArrayReturn($"select * from Rooster WHERE UserId = '{userIdArray[0]}'");
            }
            else
            {
                string sqlquery = $"select Rooster.*, Werknemers.Voornaam from Rooster INNER JOIN Werknemers ON Rooster.UserId = Werknemers.UserId";
                for (int i = 0; i < userIdArray.Length; i++)
                {
                    if (i == 0)
                    {
                        sqlquery += " WHERE ";
                    }
                    if (i > 0)
                    {
                        sqlquery += " OR ";
                    }
                    sqlquery += $"Rooster.UserId = '{userIdArray[i]}'";
                }

                return SQLConnection.ExecuteSearchQueryWithArrayReturn(sqlquery);
            }
        }
        public List<EventModel> FillEventModelList(List<string[]> events, string[] userIdArray, string rol)
        {
            List<EventModel> eventList = new List<EventModel>();
            foreach (string[] e in events)
            {
                EventModel em = new EventModel();
                em.eventId = Convert.ToInt32(e[0]);
                em.userId = e[1];
                if (rol.ToLower() == "roostermaker")
                    em.title = e[9] + " - " + e[2].ToString();
                else
                    em.title = e[2].ToString();
                em.description = e[3].ToString();
                em.startDate = Convert.ToDateTime(e[4]);
                em.endDate = Convert.ToDateTime(e[5]);
                em.themeColor = e[6].ToString();
                em.isFullDay = Convert.ToInt32(e[7]);
                em.isPending = Convert.ToBoolean(e[8]);
                eventList.Add(em);
            }
            return eventList;
        }
    }
}
