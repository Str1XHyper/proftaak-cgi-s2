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
            return QueryResult.Count > 0;
        }

        public static List<string[]> GetDiensten(int UserID)
        {
            List<string[]> returns = new List<string[]>();
            try
            {
                return SQLConnection.ExecuteSearchQueryWithArrayReturn($"Select * From Rooster Where UserId = {UserID} AND NOT ThemeColor='Verlof' ORDER BY `Start` ASC");
            }
            catch (Exception ex)
            {
                return returns;
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
                return returns;
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

        public static List<string[]> GetRequests()
        {
            List<string[]> returns = new List<string[]>();
            try
            {
                return SQLConnection.ExecuteSearchQueryWithArrayReturn($"Select * From TradeRequest ORDER BY `Start` DESC");
            }
            catch (Exception ex)
            {
                return returns;
            }
        }

        public static List<string[]> GetUsers()
        {
            List<string[]> returns = new List<string[]>();
            try
            {
                return SQLConnection.ExecuteSearchQueryWithArrayReturn($"Select * From Werknemers");
            }
            catch (Exception ex)
            {
                return returns;
            }
        }

        public static string[] FixTimeStamp(string[] requestOutput)
        {
            string date = requestOutput[2].Split(" ")[0];
            string[] startTimes = requestOutput[2].Split(" ")[1].Split(":");
            string[] endTimes = requestOutput[3].Split(" ")[1].Split(":");

            string startTime = startTimes[0] + ":" + startTimes[1];
            string endTime = endTimes[0] + ":" + endTimes[1];

            if (requestOutput[2].Contains("AM") || requestOutput[2].Contains("PM"))
            {
                string startUSTimeStamp = requestOutput[2].Split(" ")[2];
                string endUSTimeStamp = requestOutput[3].Split(" ")[2];

                startTime += $" {startUSTimeStamp}";
                endTime += $" {endUSTimeStamp}";
            }

            return new string[] {date, startTime, endTime};
        }
    }
}
