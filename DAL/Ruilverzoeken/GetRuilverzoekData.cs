using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Ruilverzoeken
{
    public class GetRuilverzoekData
    {
        public string GetLoggedInUserId(string authCode)
        {
            return SQLConnection.ExecuteSearchQuery($"Select `UserId` From `Werknemers` where AuthCode = '{authCode}'")[0];
        }

        public bool HasCorrectInfo(List<string> QueryResult)
        {
            return QueryResult.Count > 0;
        }

        public List<string[]> GetDiensten(int UserID)
        {
            try
            {
                return SQLConnection.ExecuteSearchQueryWithArrayReturn($"Select * From Rooster Where UserId = {UserID} AND NOT ThemeColor='Verlof' ORDER BY `Start` ASC");
            }
            catch (Exception ex)
            {
                return new List<string[]>();
            }
        }


        public List<string> GetNameByUIDs(int UserID)
        {
            try
            {
                return SQLConnection.ExecuteSearchQuery($"Select * From Werknemers Where UserId = {UserID}");
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        public int EntryCount(string column, string table)
        {
            try
            {
                return Convert.ToInt32(SQLConnection.ExecuteSearchQuery($"Select count({column}) From {table}")[0]);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public List<string[]> GetRequests()
        {
            try
            {
                return SQLConnection.ExecuteSearchQueryWithArrayReturn($"Select * From TradeRequest ORDER BY `Start` DESC");
            }
            catch (Exception ex)
            {
                return new List<string[]>();
            }
        }

        public List<string[]> GetUsers()
        {
            try
            {
                return SQLConnection.ExecuteSearchQueryWithArrayReturn($"Select * From Werknemers");
            }
            catch (Exception ex)
            {
                return new List<string[]>();
            }
        }

        public string[] GetRoosterData(string eventID)
        {
            return SQLConnection.ExecuteGetStringQuery($"Select * from Rooster where EventId = '{eventID}'").ToArray();
        }

        public string[] FixTimeStamp(string[] requestOutput)
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

            return new string[] { date, startTime, endTime };
        }
    }
}
