using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Classes
{
    public class AddLoginAccount
    {
        SQLConnection sQLConnection = new SQLConnection();
        public void AddLogin(string email, string ID)
        {
            string password = email.Split("@")[0] + "WW";
            sQLConnection.ExecuteNonSearchQuery($"INSERT INTO Login(UserId, Username, Password) VALUES( '{ID}', '{email.ToLower()}', AES_ENCRYPT('{password}', 'CGIKey'))");
        }

        public void ChangeLoginAdmin(string email, string password)
        {
            sQLConnection.ExecuteNonSearchQuery($"UPDATE `Login` SET `Password` = AES_ENCRYPT('{password}', 'CGIKey') WHERE `Username` = '{email.ToLower()}'");
        }
    }
}
