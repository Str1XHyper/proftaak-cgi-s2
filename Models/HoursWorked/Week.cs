using System;
using System.Collections.Generic;
using System.Text;

namespace Models.HoursWorked
{
    public class Week
    {
        public int[] StandByTime { get; set; }
        public int[] FurloughTime { get; set; }
        public int[] ActiveTime { get; set; }
        public int[] Total { get; set; }
        public Week()
        {
            StandByTime = new int[7];
            FurloughTime = new int[7];
            ActiveTime = new int[7];
            Total = new int[7];
        }
    }
}
