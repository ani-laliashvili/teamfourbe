using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVOptimization
{
    public class OptimizationResults
    {
        public class OptimizationResult
        {
            public double TotalCost { get; set; }
            public double PeakEVChargingLoad { get; set; }
            public List<EVResult> EVResults { get; set; }
            public List<CommunityLoadProfile> CommunityLoadProfiles { get; set; }
        }

        public class EVResult
        {
            public int EVId { get; set; }
            public int HouseholdId { get; set; }
            public List<EVChargeProfile> ChargeProfiles { get; set; }

            public List<double> GetStateOfChargeList()
            {
                return ChargeProfiles.Select(profile => profile.StateOfCharge).ToList();
            }

            public List<double> GetCombinedPowerSeries()
            {
                return ChargeProfiles.Select(profile => profile.ChargePower - profile.DischargePower).ToList();
            }
        }

        public class EVChargeProfile
        {
            public int Hour { get; set; }
            public double StateOfCharge { get; set; }
            public double ChargePower { get; set; }
            public double DischargePower { get; set; }
        }

        
        public class CommunityLoadProfile
        {
            public int Hour { get; set; }
            public double EVChargingPower { get; set; }
        }




    }
}
