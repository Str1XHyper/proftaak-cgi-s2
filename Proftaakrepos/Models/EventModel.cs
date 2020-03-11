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
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public EventModel(int _userId, string _title, string _description, DateTime _startDate, DateTime _endDate)
        {
            this.title = _title;
            this.description = _description;
            this.userId = _userId;
            this.startDate = _startDate;
            this.endDate = _endDate;
        }
    }
}
