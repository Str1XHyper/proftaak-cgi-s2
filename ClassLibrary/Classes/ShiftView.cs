using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using ClassLibrary.Classes;

namespace ClassLibrary.Classes
{
    public class ShiftView
    {
        public static string GetLoggedInUserId(string authCode)
        {
            return SQLConnection.ExecuteSearchQuery($"Select `UserId` From `Werknemers` where AuthCode = '{authCode}'")[0];
        }

        public static bool HasCorrectInfo(List<string> QueryResult)
        {
            return QueryResult.Count > 1;
        }

        public static List<string> GetDiensten(int UserID, int EventId)
        {
            List<string> returns = new List<string>();
            try
            {
                return SQLConnection.ExecuteSearchQuery($"Select * From Rooster Where UserId = {UserID} and EventId = {EventId}");
            }
            catch (Exception ex)
            {
                return returns;//new string[] { "Can not open connection ! " + ex.Message.ToString() };
            }
        }


        public static List<string> GetNameByUIDs(int UserID)
        {
            List<string> returns = new List<string>();
            try
            {
                return SQLConnection.ExecuteSearchQuery($"Select * From Werknemers Where UserId = {UserID}");
            }
            catch (Exception ex)
            {
                return returns;//{ "Can not open connection ! " + ex.Message.ToString() };
            }
        }

        public static int EntryCount(string column, string table)
        {
            List<string> returns = new List<string>();
            try
            {
                return Convert.ToInt32(SQLConnection.ExecuteSearchQuery($"Select count({column}) From {table}")[0]);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static List<string> GetRequests(int TradeID)
        {
            List<string> returns = new List<string>();
            try
            {
                return SQLConnection.ExecuteSearchQuery($"Select * From TradeRequest Where TradeId = {TradeID}");
            }
            catch (Exception ex)
            {
                return returns;//new string[] { "Can not open connection ! " + ex.Message.ToString() };
            }
        }
    }
}
