using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Ruilverzoeken
{
    public class AddRuilvezoekData
    {
        private SQLConnection MySql;
        public AddRuilvezoekData()
        {
            MySql = new SQLConnection();
        }

        public void AddRuilvezoek(string UserID, DateTime start, DateTime end, string EventID)
        {
            SQLConnection.ExecuteNonSearchQuery($"Insert Into `TradeRequest`(`UserIdIssuer`, `Status`, `Start`, `End`, `UserIdAcceptor`, `DisabledIDs`,`EventID`) values({UserID}, 0, '{start:yyyy/MM/dd HH:mm}', '{end:yyyy/MM/dd HH:mm}', -1, 0, '{EventID}')");
        }

        public void UpdateEventStatus(string EventID)
        {
            SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set IsPending = 1 where EventId = {EventID}");
        }

        public void BlockRequest(string UserID, int TradeID, string DisabledIds)
        {

            SQLConnection.ExecuteNonSearchQuery($"Update TradeRequest Set DisabledIds = '{DisabledIds},{UserID}' Where TradeId = {TradeID}");
        }

        public void CancelTradeRequest(string EventID)
        {
            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM `TradeRequest` WHERE `EventID` = {EventID}; UPDATE `Rooster` SET `IsPending` = 0 WHERE `EventId` = {EventID}");
        }

        public void AddTradeRequest(string UserID, int TradeID, int EventID)
        {
            string[] query = new string[] { 
                $"Update TradeRequest Set Status = 1 Where TradeId = {TradeID}", 
                $"Update TradeRequest Set UserIdAcceptor = {UserID} Where TradeId = {TradeID} ",
                $"Update Rooster Set UserId = {UserID}, IsPending = 0 Where EventId = {EventID}"
            };
            SQLConnection.ExecuteNonSearchQueryArray(query);
        }
    }
}
