using System;

namespace CoreLibrary
{
    public class EV
    {
        // Current charge of the EV in kwh
        public double CurrentCharge { get; private set; }
        public int Id { get; set; }  // EV Identifier
        public int HouseholdId { get; set; }  // ID of the household that owns the EV
        public double BatteryCapacity { get; set; }  // in kWh
        public double SoCMin { get; set; }  // Minimum State of Charge (fraction)
        public double SoCMax { get; set; }  // Maximum State of Charge (fraction)
        public double SoCEmergencyLevel { get; set; }  // Desired SoC before outage
        public bool IsAvailableForDischarge { get; set; }  // User override for upcoming travel

        // Flags to track if charging or appliance running status is active
        private bool isCharging;
        private bool isRunningEssentialAppliances;
        private bool isRunningAllAppliances;
        private readonly double consumptionRatePerMile;

        // Constructor to initialize the current charge and consumption rate
        public EV(int id, int householdId, double batteryCapacity, double initialCharge, double chargeEmergencyLevel, bool isAvailableForDischarge,
            double consumptionRatePerMile = 0.0)
        {
            if (initialCharge < 0 || initialCharge > 100)
            {
                throw new ArgumentException("Initial charge must be between 0 and 100.");
            }
            Id = id;
            HouseholdId = householdId;
            BatteryCapacity = batteryCapacity;
            SoCMax = 100.0;
            SoCMin = 0;
            SoCEmergencyLevel = chargeEmergencyLevel;
            IsAvailableForDischarge = isAvailableForDischarge;
            CurrentCharge = initialCharge;

        }

        // Method to charge the EV, increments the charge by the given amount
        public void Charge(double amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Charge amount cannot be negative.");
            }

            CurrentCharge += amount;

            if (CurrentCharge > BatteryCapacity)
            {
                CurrentCharge = BatteryCapacity; // Ensures the charge does not exceed 100%
            }
        }

        // Method to run essential appliances, decreases the charge over time
        public void RunEssentialAppliances(double amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Consumption amount cannot be negative.");
            }

            isRunningEssentialAppliances = true;
            CurrentCharge -= amount;

            if (CurrentCharge < 0)
            {
                CurrentCharge = 0; // Ensures the charge does not go below 0%
            }
        }

        // Method to run all appliances, decreases the charge over time
        public void RunAllAppliances(double amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Consumption amount cannot be negative.");
            }

            isRunningAllAppliances = true;
            CurrentCharge -= amount;

            if (CurrentCharge < 0)
            {
                CurrentCharge = 0; // Ensures the charge does not go below 0%
            }
        }

        // Method to stop running appliances
        public void StopRunningAppliances()
        {
            if (isRunningEssentialAppliances)
            {
                isRunningEssentialAppliances = false;
                Console.WriteLine("Stopped running essential appliances.");
            }

            if (isRunningAllAppliances)
            {
                isRunningAllAppliances = false;
                Console.WriteLine("Stopped running all appliances.");
            }

            if (!isRunningEssentialAppliances && !isRunningAllAppliances)
            {
                Console.WriteLine("No appliances are running.");
            }
        }

        // Method to drive the EV a certain distance
        public string Drive(double distanceInMiles)
        {
            if (distanceInMiles < 0)
            {
                return "Distance cannot be negative.";
            }

            double totalConsumption = distanceInMiles * consumptionRatePerMile;

            if (CurrentCharge < totalConsumption)
            {
                double possibleDistance = CurrentCharge / consumptionRatePerMile;
                CurrentCharge = 0;
                return $"You ran out of charge after driving {possibleDistance:F2} km.";
            }

            CurrentCharge -= totalConsumption;
            return $"You drove {distanceInMiles:F2} km. Remaining charge: {CurrentCharge:F2}%.";
        }

        // Optional: Method to return the current charge
        public double GetCurrentChargeInPercentage()
        {
            return (CurrentCharge * 100 / BatteryCapacity);
        }

        // Method to return the status of running essential appliances
        public bool IsRunningEssentialAppliances()
        {
            return isRunningEssentialAppliances;
        }

        // Method to return the status of running all appliances
        public bool IsRunningAllAppliances()
        {
            return isRunningAllAppliances;
        }
    }
}
