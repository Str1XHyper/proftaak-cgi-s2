using System;
using System.Collections.Generic;
using System.Text;
using ClassLibrary.Classes;

namespace ClassLibrary.Classes
{
    public class ChangeSettings
    {
        public static int InitSettings(string email, string emailSetting, string smssSetting)
        {
            int emailSettings;
            int smsSettings;
            List<string> userIDlist = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `Email` = '{email}'");
            int userID = Convert.ToInt32(userIDlist[0]);
            if(emailSetting.ToLower() == "ja")
            {
                emailSettings = 1;
            }
            else
            {
                emailSettings = 0;
            }

            if(smssSetting.ToLower() == "ja")
            {
                smsSettings = 1;
            }
            else
            {
                smsSettings = 0;
            }
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `Settings`(`UserId`, `ReceiveMail`, `ReceiveSMS`) VALUES ('{userID}','{emailSettings}','{smsSettings}')");
            return userID;
        }

        public static string[] getSettings(string userID)
        {
            return SQLConnection.ExecuteSearchQuery($"SELECT * FROM `Settings` WHERE `UserId` = {userID}").ToArray();
        }
    }
}
