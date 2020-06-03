using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Logic.Ruilverzoeken
{
    public class DataManager : IDataManager
    {
        private GetData getData;
        private AddData addData;

        public DataManager()
        {
            getData = new GetData();
            addData = new AddData();
        }

        public string GetLoggedInUserId(string authCode) => getData.GetLoggedInUserId(authCode);

        public bool HasCorrectInfo(List<string> QueryResult) => getData.HasCorrectInfo(QueryResult);

        public List<string[]> GetDiensten(int UserID) => getData.GetDiensten(UserID);

        public List<string> GetNameByUIDs(int UserID) => getData.GetNameByUIDs(UserID);

        public int EntryCount(string column, string table) => getData.EntryCount(column, table);

        public List<string[]> GetRequests() => getData.GetRequests();

        public List<string[]> GetUsers() => getData.GetUsers();

        public string[] GetRoosterData(string eventID) => getData.GetRoosterData(eventID);

        public void AddRequest(string EventID, string UserID) => addData.AddRequest(EventID, UserID);

        public void BlockRequest(string UserID, int TradeID, string DisabledIds) => addData.BlockRequest(UserID, TradeID, DisabledIds);

        public void CancelTradeRequest(string EventID) => addData.CancelTradeRequest(EventID);

        public void AcceptTradeRequest(string userid, int tradeid, int eventid) => addData.AcceptTradeRequest(userid, tradeid, eventid);

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
