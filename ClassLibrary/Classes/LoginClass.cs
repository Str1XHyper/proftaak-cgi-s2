using System;
using MySql.Data.MySqlClient;

namespace ClassLibrary
{
    public static class LoginClass
    {
        private static string connStr = "server=185.182.57.161;user=tijnvcd415_Proftaak; database=tijnvcd415_Proftaak;password=Proftaak";
        private enum responses { redirectHome, wrongEntry, multipleEntries, massiveError};
        public static Enum LoginUserFunction(string userName, string password)
        {
            int i = 0;
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                string sql = $"SELECT `Username`, AES_DECRYPT(Password,'CGIKey')  FROM `Login` WHERE Username='{userName.ToLower()}'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    i++;
                    if (rdr.GetString(1) == password)
                    {
                        return responses.redirectHome; //redirect to next page
                    }
                    else
                    {
                        return responses.wrongEntry;
                    }
                }
                rdr.Close();
                if (i == 0)
                {
                    return responses.wrongEntry;
                }
                else if (i != 1)
                {
                    return responses.multipleEntries;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            return responses.massiveError;
        }
    }
}
