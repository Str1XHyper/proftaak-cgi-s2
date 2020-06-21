using DAL.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Notificatie
{
    public class NotificatieHandler
    {
        private readonly SQLConnection sqlConnection;
        public NotificatieHandler()
        {
            sqlConnection = new SQLConnection();
        }
        public int[] GetAgeOfNotification(string userID, string column)
        {
            List<string[]> response = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT `{column}`, `Hoeveelheid`, `Type` from Settings WHERE UserId='{userID}'");
            if (response.Count > 0)
            {
                if(response[0][0].ToLower() == "true" || response[0][0].ToLower() == "1")
                {
                    return new int[] { Int32.Parse(response[0][1]), Int32.Parse(response[0][2])};
                }
            }
            return new int[0];
        }

        public void VerstuurAfspraakNotificatie(string userID, string eventID, int type)
        {
            List<string> eventGegevens = SQLConnection.ExecuteSearchQuery($"SELECT Start, End, IsFullDay, Subject, ThemeColor, Description FROM Rooster WHERE EventId='{eventID}'");
            List<string> werknemersGegevens = SQLConnection.ExecuteSearchQuery($"SELECT Voornaam, Email FROM Werknemers WHERE UserId='{userID}'");
            SendMail.SendNotification(werknemersGegevens[1], werknemersGegevens[0], werknemersGegevens[0], eventGegevens[3], eventGegevens[5], eventGegevens[4]);
        }

        public void PlanAfspraakNotificatie(string userID, string eventID, DateTime datum, int type)
        {
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO NotificationTasks (`TemplateID`,`UserID`,`Verzenddag`, `ExtraInformatie`, `Type`) VALUES('d-3140b8fa87aa48dcb85e16694f841293', '{userID}', '{datum.ToString("yyyy-MM-dd")}','{eventID}', '{type}')");
        }

        public List<string[]> TeVersturenNotificaties(string date)
        {
            List<string[]> response = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT `Type`, `UserId`,`ExtraInformatie`, `NotificationID` FROM NotificationTasks WHERE Verzenddag='{date}'");
            if (response.Count > 0) return response;
            else return new List<string[]>();
        }

        public void DeleteTask(string id)
        {
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM NotificationTasks WHERE NotificationID='{id}'");
        }
    }
}
