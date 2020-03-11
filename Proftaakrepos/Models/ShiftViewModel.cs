using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Proftaakrepos.Models
{
    public class ShiftViewModel
    {
        public string[] GetNameByUID(int UserID)
        {
            MySqlConnection cnn;
            string connetionString = "server=185.182.57.161;database=tijnvcd415_Proftaak;uid=tijnvcd415_Proftaak;pwd=Proftaak;";
            cnn = new MySqlConnection(connetionString);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = $"Select * From Werknemers Where UserId = {UserID}";
            try
            {
                string[] returnStrings = new string[12];
                cnn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        returnStrings[i] = reader[i].ToString();
                    }
                    break;
                }
                cnn.Close();
                return returnStrings;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return new string[] { "Can not open connection ! " + ex.Message.ToString() };
            }
        }

        public string[] GetRequest(int TradeID)
        {
            MySqlConnection cnn;
            string connetionString = "server=185.182.57.161;database=tijnvcd415_Proftaak;uid=tijnvcd415_Proftaak;pwd=Proftaak;";
            cnn = new MySqlConnection(connetionString);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = $"Select * From TradeRequest Where TradeId = {TradeID}";
            try
            {
                string[] returnStrings = new string[7];
                cnn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        returnStrings[i] = reader[i].ToString();
                    }
                    break;
                }
                cnn.Close();
                return returnStrings;
            }
            catch (Exception ex)
            {
                //"Can not open connection ! " + ex.Message.ToString()
                return null;
            }
        }
    }
}
