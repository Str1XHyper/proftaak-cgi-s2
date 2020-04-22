using System;
using System.Collections.Generic;
using ClassLibrary.Classes;
using MySql.Data.MySqlClient;

namespace ClassLibrary
{
    public class LoginClass
    {
        private enum responses { redirectHome, wrongEntry, multipleEntries, massiveError };
        public static Enum LoginUserFunction(string userName, string password)
        {
            string sql;
            List<string> passwords;
            List<string> usernames;
            if (userName != null)
            {
                sql = $"SELECT `Username` FROM `Login` WHERE Username='{userName.ToLower()}'";
                usernames = SQLConnection.ExecuteSearchQuery(sql);
                sql = $"SELECT AES_DECRYPT(Password,'CGIKey')  FROM `Login` WHERE Username='{userName.ToLower()}'";
                passwords = SQLConnection.ExecuteGetStringQuery(sql);
                if (usernames.Count == 1)
                {
                    string retrievedPassword = passwords[0];
                    if (password == retrievedPassword)
                    {
                        //SendMail.Execute().Wait();
                        return responses.redirectHome;
                    }
                    else
                    {
                        return responses.wrongEntry;
                    }
                }
                else if (usernames.Count != 2)
                {
                    return responses.wrongEntry;
                }
                else if (usernames.Count == 2)
                {
                    return responses.multipleEntries;
                }

                return responses.massiveError;
            }
            else
            {
                return responses.wrongEntry;
            }
        }
    }
}