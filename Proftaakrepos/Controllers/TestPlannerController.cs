using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Agenda;
using Ubiety.Dns.Core.Records.NotUsed;

namespace Proftaakrepos.Controllers
{
    public class TestPlannerController : Controller
    {
        public List<ParseableEventModel> FetchAllEvents(string userIds)
        {
            if(userIds == "" || userIds == null)
            {
                return new List<ParseableEventModel>();
            }

            // Encode to prevent SQL Injection
            string[] uids = new string[userIds.Split(',').Length];
            for(int i = 0; i < userIds.Split(",").Length; i++)
            {
                uids[i] = HtmlEncoder.Default.Encode(userIds.Split(',')[i]);
            }

            // Create sql query
            string sqlQuery = $"Select * from `Rooster` WHERE `UserId` = '{uids[0]}'";
            if(uids.Length > 1)
            {
                for (int i = 1; i < uids.Length; i++)
                {
                    sqlQuery += $"OR `UserId` = '{uids[i]}'";
                }
            }

            // Get events
            List<string[]> result = SQLConnection.ExecuteSearchQueryWithArrayReturn(sqlQuery);

            // Get colors
            List<string> colors = SQLConnection.ExecuteSearchQuery("SELECT * FROM `ColorScheme`");
            if(colors.Count < 1)
            {
                colors = new List<string>();
                // Standby
                colors.Add("#3B5A6F");
                // Incidenten
                colors.Add("#353B45");
                // Pauze
                colors.Add("#828A87");
                // Verlof
                colors.Add("#830101");
            }

            // Put result into models
            String[] c = { "Stand-by", "Incidenten", "Pauze", "Verlof" };
            List<ParseableEventModel> returnList = new List<ParseableEventModel>();
            foreach(string[] row in result)
            {
                int index = Array.IndexOf(c, row[6]);
                ParseableEventModel em = new ParseableEventModel
                {
                    id = Convert.ToInt32(row[0]),
                    title = row[2],
                    start = DateTime.Parse(row[4]),
                    end = DateTime.Parse(row[5]),
                    backgroundColor = colors[index],
                    allDay = Convert.ToBoolean(Convert.ToInt32(row[7])),
                    description = row[3],
                    borderColor = "#010203"
                };
                returnList.Add(em);
            }

            // return parsable model for Full Calendar
            return returnList;
        }

        public void UpdateEvent(DateTime start, DateTime end, string eventid, bool allday)
        {
            // Encode to prevent SQL Injection
            string startTime = HtmlEncoder.Default.Encode(start.ToString("yyyy/MM/dd HH:mm"));
            string endTime = HtmlEncoder.Default.Encode(end.ToString("yyyy/MM/dd HH:mm"));
            eventid = HtmlEncoder.Default.Encode(eventid);
            string alldaystring = HtmlEncoder.Default.Encode(allday.ToString());
            allday = Boolean.Parse(alldaystring);

            // Update event
            SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set Start = '{startTime}',End = '{endTime}',IsFullDay = '{Convert.ToInt32(allday)}' Where EventId = '{eventid}'");
        }
    }
}