using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace ClassLibrary.Classes
{
    public class SQLConnection
    {
        private List<string> values = new List<string>();
        private MySqlDataReader reader;
        private MySqlConnection CreateConnection(string IP, string Database, string UserName, string Password)
        {
            MySqlConnection cnn;
            string connetionString = $"server={IP};database={Database};uid={UserName};pwd={Password};";
            cnn = new MySqlConnection(connetionString);
            return cnn;
        }

        public List<string> ExecuteSearchQuery(string query)
        {
            values.Clear();
            MySqlConnection cnn = CreateConnection("185.182.57.161", "tijnvcd415_Proftaak", "tijnvcd415_Proftaak", "Proftaak");
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = query;
            cmd.Connection = cnn;
            cnn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            values.Clear();
            while (reader.Read())
            {
                for(int i = 0; i < reader.FieldCount; i++)
                {
                    values.Add(reader[i].ToString());
                }
            }
            cnn.Close();
            return values;
        }

        public void ExecuteNonSearchQuery(string query)
        {
            MySqlConnection cnn = CreateConnection("185.182.57.161", "tijnvcd415_Proftaak", "tijnvcd415_Proftaak", "Proftaak");
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = query;
            cmd.Connection = cnn;
            cnn.Open();
            cmd.ExecuteNonQuery();
            cnn.Close();
        }

        public List<string> ExecuteGetStringQuery(string query)
        {
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
