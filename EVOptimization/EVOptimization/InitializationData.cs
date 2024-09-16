using CoreLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVOptimization
{
    // Class to store initialization data
    public class InitializationData
    {
        public int NumTimeSlots { get; set; }
        public List<Household> Households { get; set; }
        public List<EV> EVs { get; set; }
        public List<Appliance> Appliances { get; set; }
        public double[] P_Price { get; set; }
        public Outage Outage { get; set; }
    }
}
