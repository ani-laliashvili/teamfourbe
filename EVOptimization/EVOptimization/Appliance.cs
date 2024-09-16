using System;
using System.Collections.Generic;

namespace EVOptimization
{
    public class Appliance
    {
        // Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public double PowerConsumption { get; set; }

        // Constructor
        public Appliance(int id, string name, double powerConsumption)
        {
            Id = id;
            Name = name;
            PowerConsumption = powerConsumption;
        }

        // Static Function to create sample appliances
        public static List<Appliance> CreateAppliances()
        {
            List<Appliance> appliances = new List<Appliance>
            {
                new Appliance(1, "Fridge", 0.2), // kW
                new Appliance(2, "Lights", 0.1), // kW
                new Appliance(3, "HVAC", 2.0)    // kW
            };

            return appliances;
        }

        // Function to calculate total appliance power for a household at time h
        public static double GetTotalAppliancePower(Household household, List<Appliance> appliances, int h, int[] OutagePeriod)
        {
            bool isOutage = Array.Exists(OutagePeriod, hour => hour == h);

            double totalPower = 0.0;

            foreach (int applianceId in household.Appliances)
            {
                Appliance appliance = appliances.Find(a => a.Id == applianceId);
                if (isOutage)
                {
                    // During outage, only essential appliances are on
                    if (household.EssentialAppliances.Contains(applianceId))
                    {
                        totalPower += appliance.PowerConsumption;
                    }
                }
                else
                {
                    totalPower += appliance.PowerConsumption;
                }
            }

            return totalPower;
        }
    }
}