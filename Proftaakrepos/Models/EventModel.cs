using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proftaakrepos.Models
{
    public class EventModel
    {
        public int userId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; }
        public DateTime time { get; set; }
        public EventModel(string _title, string _description, int _userId)
        {
            title = _title;
            description = _description;
            userId = _userId;
        }
    }
}
