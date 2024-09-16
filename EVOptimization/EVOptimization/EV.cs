using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVOptimization
{
    public class EV
    {
        // Properties of the EV class
        public int Id { get; set; }  // EV Identifier
        public int HouseholdId { get; set; }  // ID of the household that owns the EV
        public double BatteryCapacity { get; set; }  // in kWh
        public double SoCMin { get; set; }  // Minimum State of Charge (fraction)
        public double SoCMax { get; set; }  // Maximum State of Charge (fraction)
        public double SoCInitial { get; set; }  // Initial SoC at the beginning of the period
        public double SoCEmergencyLevel { get; set; }  // Desired SoC before outage
        public bool IsAvailableForDischarge { get; set; }  // User override for upcoming travel

        public static List<EV> CreateEVs(List<Household> households)
        {
            List<EV> EVs = new List<EV>();

            // EV 1 for Household 1
            EVs.Add(new EV
            {
                Id = 1,
                HouseholdId = 1,
                BatteryCapacity = 60.0, // kWh
                SoCMin = 0.2,
                SoCMax = 0.9,
                SoCInitial = 0.5,
                SoCEmergencyLevel = 0.8,
                IsAvailableForDischarge = true
            });

            // EV 2 for Household 2
            EVs.Add(new EV
            {
                Id = 2,
                HouseholdId = 2,
                BatteryCapacity = 50.0, // kWh
                SoCMin = 0.2,
                SoCMax = 0.9,
                SoCInitial = 0.6,
                SoCEmergencyLevel = 0.7,
                IsAvailableForDischarge = false // User override
            });

            return EVs;
        }
    }
}
