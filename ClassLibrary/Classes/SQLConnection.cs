using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace ClassLibrary.Classes
{
    public class SQLConnection
    {
        private MySqlDataReader reader;
        private static MySqlConnection CreateConnection()
        {
            MySqlConnection cnn;
            //string connetionString = $"server=bier-1.democgi.com;database=PlannerApplicatie;uid=nova;pwd=AkXxYFSD03oFLHmV;";
            string connetionString = $"server=185.182.57.161;database=tijnvcd415_Proftaak;uid=tijnvcd415_Proftaak;pwd=Proftaak;";
            cnn = new MySqlConnection(connetionString);
            return cnn;
        }

        public static List<string> ExecuteSearchQuery(string query)
        {
            List<string> values = new List<string>();
            MySqlConnection cnn = CreateConnection();
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
        public static List<string>[] ExecuteSearchQueryArray(string[] query)
        {
            List<string>[] values = new List<string>[query.Length];
            MySqlConnection cnn = CreateConnection();
            MySqlCommand[] cmd = new MySqlCommand[query.Length];
            for (int i = 0; i < cmd.Length; i++)
            {
                cmd[i] = new MySqlCommand();
                cmd[i].CommandText = query[i];
                cmd[i].Connection = cnn;
            }
            cnn.Open();
            for(int i = 0; i < cmd.Length; i++)
            {
                MySqlDataReader reader = cmd[i].ExecuteReader();
                values[i].Clear();
                try
                {
                    while (reader.Read())
                    {
                        for (int j = 0; j < reader.FieldCount; j++)
                        {
                            values[i].Add(reader[j].ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    string eString = e.ToString();
                }
            }
            
            cnn.Close();
            return values;
        }

        public static void ExecuteNonSearchQuery(string query)
        {
            MySqlConnection cnn = CreateConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = query;
            cmd.Connection = cnn;
            cnn.Open();
                cmd.ExecuteNonQuery();
            cnn.Close();
        }

        public static void ExecuteNonSearchQueryArray(string[] query)
        {
            MySqlConnection cnn = CreateConnection();

            MySqlCommand[] cmd = new MySqlCommand[query.Length];
            for (int i = 0; i<cmd.Length; i++)
            {
                cmd[i] = new MySqlCommand();
                cmd[i].CommandText = query[i];
                cmd[i].Connection = cnn;
            }
            cnn.Open();
            for(int i = 0; i<cmd.Length; i++)
            {
                cmd[i].ExecuteNonQuery();
            }
            cnn.Close();
        }

        public static List<string> ExecuteGetStringQuery(string query)
        {
            List<string> values = new List<string>();
            MySqlConnection cnn = CreateConnection();
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
