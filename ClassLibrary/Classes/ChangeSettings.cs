using System;
using System.Collections.Generic;
using System.Text;
using ClassLibrary.Classes;

namespace ClassLibrary.Classes
{
    public class ChangeSettings
    {
        SQLConnection sQLConnection = new SQLConnection();
        private int emailSettings;
        private int smsSettings;
        public void InitSettings(string email, string emailSetting, string smssSetting)
        {
            List<string> userIDlist = sQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `Email` = '{email}'");
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
            sQLConnection.ExecuteNonSearchQuery($"INSERT INTO `Settings`(`UserId`, `ReceiveMail`, `ReceiveSMS`) VALUES ('{userID}','{emailSettings}','{smsSettings}')");
        }
    }
}
