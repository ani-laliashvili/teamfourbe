using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    public class Outage
    {
        public int HoursFromNowStart { get; private set; }  // Hours from now until the outage starts
        public int HoursFromNowEnd { get; private set; }  // Duration of the outage in hours
        public string Reason { get; private set; }  // Reason for the outage

        // Constructor to initialize the outage
        public Outage(int hoursFromNowStart, int hoursFromNowEnd, string reason = "")
        {
            HoursFromNowStart = hoursFromNowStart;
            HoursFromNowEnd = hoursFromNowEnd;
            Reason = reason;
        }

        // Method to display outage information
        public string GetOutageInfo()
        {
            return $"Outage starts in {HoursFromNowStart} hours and lasts for {HoursFromNowEnd} hours. Reason: {Reason}.";
        }
    }

}
