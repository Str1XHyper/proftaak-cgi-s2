using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;

namespace DAL
{
    public class SQLConnection
    {
        private static MySqlConnection CreateConnection()
        {
            MySqlConnection cnn;
            //for debugging
            //string connetionString = $"server=bier-1.democgi.com;database=PlannerApplicatie;uid=nova;pwd=AkXxYFSD03oFLHmV;";
            //for release
            //string connetionString = $"server=localhost;database=PlannerApplicatie;uid=nova;pwd=AkXxYFSD03oFLHmV;";
            //for anti-
            string connetionString = $"server=185.182.56.248;database=bartvur381_proftaak;uid=bartvur381_proftaak;pwd=KjYD1zjZ;";
            cnn = new MySqlConnection(connetionString);
            return cnn;
        }
        public static List<string[]> ExecuteSearchQueryWithArrayReturn(string query)
        {
            string[] tempStrArr;
            List<string[]> values = new List<string[]>();
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
                    tempStrArr = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        tempStrArr[i] = reader[i].ToString();
                    }
                    values.Add(tempStrArr);
                }
            }
            catch (Exception e)
            {
                string eString = e.ToString();
            }
            cnn.Close();
            return values;
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
            catch (Exception e)
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
            for (int i = 0; i < cmd.Length; i++)
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

        public static bool ExecuteNonSearchQuery(string query)
        {
            MySqlConnection cnn = CreateConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = query;
            cmd.Connection = cnn;
            cnn.Open();
            try
            {
                cmd.ExecuteNonQuery();
            } 
            catch(Exception ex)
            {
                string test = ex.ToString();
                return false;
            }
            cnn.Close();
            return true;
        }

        public static bool ExecuteNonSearchQueryArray(string[] query)
        {
            MySqlConnection cnn = CreateConnection();

            MySqlCommand[] cmd = new MySqlCommand[query.Length];
            for (int i = 0; i < cmd.Length; i++)
            {
                cmd[i] = new MySqlCommand();
                cmd[i].CommandText = query[i];
                cmd[i].Connection = cnn;
            }
            cnn.Open();
            try
            {
                for (int i = 0; i < cmd.Length; i++)
                {
                    cmd[i].ExecuteNonQuery();
                }
            }
            catch
            {
                return false;
            }
            cnn.Close();
            return true;
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
