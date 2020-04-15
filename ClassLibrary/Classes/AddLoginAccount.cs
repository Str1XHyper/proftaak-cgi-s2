using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Classes
{
    public class AddLoginAccount
    {
        public static void AddLogin(string naam, string ID, string email)
        {
            string password = naam + "WW";
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO Login(UserId, Username, Password) VALUES( '{ID}', '{email.ToLower()}', AES_ENCRYPT('{password}', 'CGIKey'))");
        }

        public static void ChangeLoginAdmin(string email, string password)
        {
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Login` SET `Password` = AES_ENCRYPT('{password}', 'CGIKey') WHERE `Username` = '{email.ToLower()}'");
        }
    }
}
