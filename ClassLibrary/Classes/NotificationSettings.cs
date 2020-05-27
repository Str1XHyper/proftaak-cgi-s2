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
    }
}
