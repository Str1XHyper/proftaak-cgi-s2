using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace ClassLibrary.Classes
{
    public class NotificationSettings
    {
        public void PasSettingsAan(string TypeOfAge, int ValueOfNoti, string userID)
        {
            switch (Convert.ToInt32(TypeOfAge))
            {
                case 0: //DIRECT
                    SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `Hoeveelheid`='0', `Type`='0' WHERE `UserId`='{userID}'");
                    break;
                case 1: //DAGEN
                    SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `Hoeveelheid`='{ValueOfNoti}', `Type`='1' WHERE `UserId`='{userID}'");
                    break;
                case 2: //WEEEEEEEEEEEEEEEEEEEEEEEEEEEEEKEN
                    SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `Hoeveelheid`='{ValueOfNoti}', `Type`='2' WHERE `UserId`='{userID}'");
                    break;
                case 3: //MAANDEN
                    SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `Hoeveelheid`='{ValueOfNoti}', `Type`='3' WHERE `UserId`='{userID}'");
                    break;
                default: //waduhek
                    throw new System.ArgumentException("waduhek", "bananaman");
            }
        }

        public void SendRoosterNotifcation(string userID, string eventID)
        {
            List<string> response = SQLConnection.ExecuteSearchQuery($"SELECT ReceiveMail, ReceiveSMS, ReceiveWhatsApp, Type, Hoeveelheid FROM Settings WHERE UserId='{userID}'");
            if(response.Count > 0)
            {
                if (response[0].ToLower() == "true")
                {
                    //wilt email ontvangen

                    List<string> eventGegevens = SQLConnection.ExecuteSearchQuery($"SELECT Start, End, IsFullDay, Subject, ThemeColor, Description FROM Rooster WHERE EventId='{eventID}'");
                    List<string> werknemersGegevens = SQLConnection.ExecuteSearchQuery($"SELECT Voornaam, Email FROM Werknemers WHERE UserId='{userID}'");
                    SendMail.SendNotification(werknemersGegevens[1], werknemersGegevens[0], werknemersGegevens[0], eventGegevens[3], eventGegevens[5], eventGegevens[4]);
                }
            }
        }
    }
}