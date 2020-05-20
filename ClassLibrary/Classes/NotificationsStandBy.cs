﻿using Models;
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
            return false;
        }

        public static List<string[]> GetStandByEmployees()
        {
            List<string[]> employeeData = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT UserId,Start,End FROM Rooster");
            int standbyUserCount = 0;
            string sqlquery = ($"SELECT Distinct Email,Voornaam FROM Werknemers WHERE UserId='");
            foreach (string[] data in employeeData)
            {
                if (DateTime.Now >= Convert.ToDateTime(data[1]) && Convert.ToDateTime(data[2]) >= DateTime.Now)
                {
                    standbyUserCount++;
                    sqlquery += data[0] + "' OR UserId='";
                }
            }
            if (standbyUserCount > 0)
            {
                sqlquery.Substring(0, sqlquery.Length - 12);
                return SQLConnection.ExecuteSearchQueryWithArrayReturn(sqlquery);
            }
            return null;
        }
    }
}