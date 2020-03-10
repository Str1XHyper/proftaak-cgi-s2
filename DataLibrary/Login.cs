using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using DataLibrary.Models;

namespace DataLibrary
{
    class Login
    {
        private static string connStr = "server=185.182.57.161;user=tijnvcd415_Proftaak; database=tijnvcd415_Proftaak;password=Proftaak";
        private enum responses {redirectHome, wrongEntry, multipleEntries, massiveError};
        public Enum LoginUserFunction(LoginModel model)
        {
            int i = 0;
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = $"SELECT `Username`, AES_DECRYPT(Password,'CGIKey')  FROM `Login` WHERE Username='{model.Username.ToLower()}'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    i++;
                    if (rdr.GetString(1) == model.Password)
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
