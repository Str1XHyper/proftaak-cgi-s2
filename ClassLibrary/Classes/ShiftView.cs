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
        public string GetLoggedInUserId(string authCode)
        {
            return sqlConnection.ExecuteSearchQuery($"Select `UserId` From `Werknemers` where AuthCode = '{authCode}'")[0];
        }

        public bool HasCorrectInfo(string[] QueryResult)
        {
            return QueryResult.Length > 1;
        }

        public string[] GetDiensten(int UserID, int EventId)
        {
            try
            {
                return sqlConnection.ExecuteSearchQuery($"Select * From Rooster Where UserId = {UserID} and EventId = {EventId}");
            }
            catch (Exception ex)
            {
                return new string[] { "Can not open connection ! " + ex.Message.ToString() };
            }
        }


        public string[] GetNameByUIDs(int UserID)
        {
            try
            {
                return sqlConnection.ExecuteSearchQuery($"Select * From Werknemers Where UserId = {UserID}");
            }
            catch (Exception ex)
            {
                return new string[] { "Can not open connection ! " + ex.Message.ToString() };
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

        public string[] GetRequests(int TradeID)
        {
            try
            {
                return sqlConnection.ExecuteSearchQuery($"Select * From TradeRequest Where TradeId = {TradeID}");
            }
            catch (Exception ex)
            {
                return new string[] { "Can not open connection ! " + ex.Message.ToString() };
            }
        }
    }
}
