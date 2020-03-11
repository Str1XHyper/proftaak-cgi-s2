using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using ClassLibrary.Classes;

namespace ClassLibrary.Classes
{
    public class ShiftView
    {
        private SQLConnection sqlConnection = new SQLConnection();
        List<string> returns;
        public string GetLoggedInUserId(string authCode)
        {
            return sqlConnection.ExecuteSearchQuery($"Select `UserId` From `Werknemers` where AuthCode = '{authCode}'")[0];
        }

        public bool HasCorrectInfo(string[] QueryResult)
        {
            return QueryResult.Length > 1;
        }

        public List<string> GetDiensten(int UserID, int EventId)
        {
            try
            {
                return sqlConnection.ExecuteSearchQuery($"Select * From Rooster Where UserId = {UserID} and EventId = {EventId}");
            }
            catch (Exception ex)
            {
                return returns;//new string[] { "Can not open connection ! " + ex.Message.ToString() };
            }
        }


        public List<string> GetNameByUIDs(int UserID)
        {
            try
            {
                return sqlConnection.ExecuteSearchQuery($"Select * From Werknemers Where UserId = {UserID}");
            }
            catch (Exception ex)
            {
                return returns;//{ "Can not open connection ! " + ex.Message.ToString() };
            }
        }

        public int EntryCount(string column, string table)
        {
            try
            {
                return Convert.ToInt32(sqlConnection.ExecuteSearchQuery($"Select count({column}) From {table}")[0]);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public List<string> GetRequests(int TradeID)
        {
            try
            {
                return sqlConnection.ExecuteSearchQuery($"Select * From TradeRequest Where TradeId = {TradeID}");
            }
            catch (Exception ex)
            {
                return returns;//new string[] { "Can not open connection ! " + ex.Message.ToString() };
            }
        }
    }
}
