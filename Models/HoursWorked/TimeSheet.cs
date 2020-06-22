using System;
using System.Collections.Generic;
using System.Text;

namespace Models.HoursWorked
{
    public class TimeSheet
    {
        public List<string> Dates { get; set; }
        public List<string> Start { get; set; }
        public List<string> End { get; set; }
        public List<string> OverTime { get; set; }
        public List<string> Type { get; set; }
    }
}
