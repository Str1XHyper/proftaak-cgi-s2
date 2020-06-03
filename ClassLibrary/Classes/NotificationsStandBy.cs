using Models;
using Models.Incidenten;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Classes
{
    public class NotificationsStandBy
    {
        public static bool NotifyStandyBy(AddIncidentModel model)
        {
            List<string[]> users = GetStandByEmployees();
            if(users.Count > 0)
            {
                foreach (string[] user in users)
                {
                    var email = user[0];
                    SendMail.SendEmployeeIncident("bartdgp@outlook.com","Bart Vermeulen", model.IncidentOmschrijving, model.IncidentNaam);
                    SendMail.SendKlantIncident("bartdgp@outlook.com", "Bart Vermeulen", model.IncidentOmschrijving, model.IncidentNaam);
                }
            } else
            {
                return false;
            }
            return true;
        }

        public static void NotifySolved(IncidentMailModel model)
        {
            SendMail.SendEmployeeIncidentSolved("BartDGP@outlook.com", "Bart Vermeulen", model.Beschrijving, model.Naam);
            SendMail.SendKlantIncidentSolved("BartDGP@outlook.com", "Bart Vermeulen", model.Beschrijving, model.Naam);
        }

        public static List<string[]> GetStandByEmployees()
        {
            List<string[]> roosterData = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT UserId,Start,End FROM Rooster");
            List<string[]> userData = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT UserId,Email,Voornaam FROM Werknemers");
            List<string[]> users = new List<string[]>();

            foreach(string[] roosterEvent in roosterData)
            {
                DateTime start = DateTime.Parse(roosterEvent[1]);
                DateTime end = DateTime.Parse(roosterEvent[2]);
                if (DateTime.Compare(start, DateTime.Now) < 0 && DateTime.Compare(end, DateTime.Now) > 0)
                {
                    bool userInList = false;
                    foreach(string[] userInfo in userData)
                    {
                        if(userInfo[0] == roosterEvent[0])
                        {
                            if (users.Count > 0)
                            {
                                foreach (string[] user in users)
                                {
                                    if(user[0] == userInfo[0])
                                    {
                                        userInList = true;
                                        break;
                                    }
                                }

                                if (userInList)
                                {
                                    break;
                                } else
                                {
                                    users.Add(userInfo);
                                }
                            }
                            else
                            {
                                users.Add(userInfo);
                            }
                        }
                    }
                }
            }

            return users;
        }
    }
}
