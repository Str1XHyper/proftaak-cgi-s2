using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace ClassLibrary.Classes
{
    public class SQLConnection
    {
        private MySqlDataReader reader;
        private static MySqlConnection CreateConnection(string IP, string Database, string UserName, string Password)
        {
            MySqlConnection cnn;
            string connetionString = $"server={IP};database={Database};uid={UserName};pwd={Password};";
            cnn = new MySqlConnection(connetionString);
            return cnn;
        }

        public static List<string> ExecuteSearchQuery(string query)
        {
            List<string> values = new List<string>();
            MySqlConnection cnn = CreateConnection("185.182.57.161", "tijnvcd415_Proftaak", "tijnvcd415_Proftaak", "Proftaak");
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = query;
            cmd.Connection = cnn;
            cnn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            values.Clear();
            try
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        values.Add(reader[i].ToString());
                    }
                }
            }
            catch(Exception e)
            {
                string eString = e.ToString();
            }
            cnn.Close();
            return values;
        }

        public static void ExecuteNonSearchQuery(string query)
        {
            MySqlConnection cnn = CreateConnection("185.182.57.161", "tijnvcd415_Proftaak", "tijnvcd415_Proftaak", "Proftaak");
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = query;
            cmd.Connection = cnn;
            cnn.Open();
            cmd.ExecuteNonQuery();
            cnn.Close();
        }

        public static List<string> ExecuteGetStringQuery(string query)
        {
            List<string> values = new List<string>();
            MySqlConnection cnn = CreateConnection("185.182.57.161", "tijnvcd415_Proftaak", "tijnvcd415_Proftaak", "Proftaak");
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = query;
            cmd.Connection = cnn;
            cnn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            values.Clear();
            while (reader.Read())
            {
                
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    values.Add(reader.GetString(i));
                }
            }
            cnn.Close();
            return values;
        }
    }
}
