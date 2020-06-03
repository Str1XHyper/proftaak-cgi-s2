using DAL.Ruilverzoeken;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Ruilverzoeken
{
    class AddData
    {
        private GetData getData;
        private AddRuilvezoekData addData;
        public AddData()
        {
            getData = new GetData();
            addData = new AddRuilvezoekData();
        }
        public void AddRequest(string EventID, string UserID)
        {
            string[] roosterData = getData.GetRoosterData(EventID);

            DateTime start = DateTime.Parse(roosterData[4]);
            DateTime end = DateTime.Parse(roosterData[5]);
            DateTime startnew = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second);
            DateTime endnew = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second);

            addData.AddRuilvezoek(UserID, startnew, endnew, EventID);
            addData.UpdateEventStatus(EventID);
        }

        public void BlockRequest(string UserID, int TradeID, string DisabledIds) => addData.BlockRequest(UserID, TradeID, DisabledIds);
        public void CancelTradeRequest(string eventid) => addData.CancelTradeRequest(eventid);
        public void AcceptTradeRequest(string userid, int tradeid, int eventid) => addData.AddTradeRequest(userid, tradeid, eventid);
    }
}
