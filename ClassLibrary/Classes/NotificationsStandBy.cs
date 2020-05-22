using Models;
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
            string mailBody1 = $"<div div class='container mt-5'><div class='card'><div class='card-header bg-dark text-white'><div>Incident: {model.IncidentNaam}</div></div><div class='Card-body'><h5 class='card-title'>Er is een incident<br /> Omschrijving: {model.IncidentOmschrijving}</h5></div></div></div>";
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

        public static void NotifySolved()
        {
            SendMail.SendEmployeeIncidentSolved("BartDGP@outlook.com", "Bart Vermeulen", "Lorem Ipsum Test Titel", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec sagittis dolor quis tellus viverra, eu viverra erat vehicula. Aliquam mollis.");
            SendMail.SendKlantIncidentSolved("BartDGP@outlook.com", "Bart Vermeulen", "Lorem Ipsum Test Titel", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec sagittis dolor quis tellus viverra, eu viverra erat vehicula. Aliquam mollis.");
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

            //List<string[]> employeeData = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT UserId,Start,End FROM Rooster");
            //int standbyUserCount = 0;
            //string sqlquery = ($"SELECT Distinct Email,Voornaam FROM Werknemers WHERE UserId='");
            //foreach (string[] data in employeeData)
            //{
            //    if (DateTime.Now >= Convert.ToDateTime(data[1]) && Convert.ToDateTime(data[2]) >= DateTime.Now)
            //    {
            //        standbyUserCount++;
            //        sqlquery += data[0] + "' OR UserId='";
            //    }
            //}
            //if (standbyUserCount > 0)
            //{
            //    sqlquery.Substring(0, sqlquery.Length - 12);
            //    return SQLConnection.ExecuteSearchQueryWithArrayReturn(sqlquery);
            //}
            //return new List<string[]>();
        }
    }
}
