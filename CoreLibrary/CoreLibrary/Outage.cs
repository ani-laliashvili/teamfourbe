using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    public class Outage
    {
        public DateTime StartTime { get; set; } // Outage start time
        public DateTime EndTime { get; set; } // Outage end time
        public string Reason { get; set; } = ""; // Optional reason for the outage
    }
}
