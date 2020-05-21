using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Agenda
{
    public class ParseableEventModel
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public bool allDay { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string backgroundColor { get; set; }
        public string description { get; set; }
        public string borderColor { get; set; }
    }
}
