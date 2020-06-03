using System.Collections.Generic;

namespace Logic.Ruilverzoeken
{
    public interface IDataManager
    {
        void AcceptTradeRequest(string userid, int tradeid, int eventid);
        void AddRequest(string EventID, string UserID);
        void BlockRequest(string UserID, int TradeID, string DisabledIds);
        void CancelTradeRequest(string EventID);
        int EntryCount(string column, string table);
        string[] FixTimeStamp(string[] requestOutput);
        List<string[]> GetDiensten(int UserID);
        string GetLoggedInUserId(string authCode);
        List<string> GetNameByUIDs(int UserID);
        List<string[]> GetRequests();
        string[] GetRoosterData(string eventID);
        List<string[]> GetUsers();
        bool HasCorrectInfo(List<string> QueryResult);
    }
}