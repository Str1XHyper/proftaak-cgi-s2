using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Classes
{
    public class NotificationsStandBy
    {
        public static async Task<bool> NotifyStandyBy(AddIncidentModel model)
        {
            string mailBody1 = $"<div div class='container mt-5'><div class='card'><div class='card-header bg-dark text-white'><div>Incident: {model.IncidentNaam}</div></div><div class='Card-body'><h5 class='card-title'>Er is een incident<br /> Omschrijving: {model.IncidentOmschrijving}</h5></div></div></div>";
            List<string[]> users = GetStandByEmployees();
            if(users.Count > 0)
            {
                foreach (string[] user in users)
                {
                    var email = user[0];
                    bool succeeded = await SendMail.Execute("Incident", "tijn.vanveghel@student.fontys.nl", mailBody1, "Error sending email");
                    if (!succeeded)
                    {
                        Console.WriteLine("Error while sending email");
                    }
                    else
                    {
                        Console.WriteLine($"Email sent to {user[1]}");
                    }
                }
            } else
            {
                return false;
            }
            return true;
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
                                    users.Add(userInfo);
                                }
                                if (userInList)
                                {
                                    break;
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
