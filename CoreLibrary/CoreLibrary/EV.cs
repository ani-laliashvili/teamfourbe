using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    public class EV
    {
        // Current charge of the EV in percentage (0 to 100)
        public double CurrentCharge { get; private set; }
        public int Id { get; set; }  // EV Identifier
        public int HouseholdId { get; set; }  // ID of the household that owns the EV
        public double BatteryCapacity { get; set; }  // in kWh
        public double SoCMin { get; set; }  // Minimum State of Charge (fraction)
        public double SoCMax { get; set; }  // Maximum State of Charge (fraction)
        public double SoCEmergencyLevel { get; set; }  // Desired SoC before outage
        public bool IsAvailableForDischarge { get; set; }  // User override for upcoming travel

        // Constructor to initialize the current charge and consumption rate
        public EV(int id, int householdId, double batteryCapacity, double initialCharge, double chargeEmergencyLevel, bool isAvailableForDischarge)
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

        // Method to charge the EV, increments the charge by the given amount
        public void Charge(double amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Charge amount cannot be negative.");
            }

            CurrentCharge += amount;

            if (CurrentCharge > 100)
            {
                CurrentCharge = 100; // Ensures the charge does not exceed 100%
            }
        }

        // Method to discharge the EV, decreases the charge by the given amount
        public void Discharge(double amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Discharge amount cannot be negative.");
            }

            CurrentCharge -= amount;

            if (CurrentCharge < 0)
            {
                CurrentCharge = 0; // Ensures the charge does not go below 0%
            }
        }

        // Method to send charge to another component in the home grid
        public void SendChargeTo(double amount, Appliance component)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount to send cannot be negative.");
            }

            if (CurrentCharge < amount)
            {
                throw new InvalidOperationException("Not enough charge to send.");
            }

            CurrentCharge -= amount;
            component.ReceiveCharge(amount);

            Console.WriteLine($"Sent {amount}% charge to {component.Name}. Remaining charge: {CurrentCharge}%.");
        }

        // Method to charge the EV over time asynchronously
        public async Task<string> ChargeOverTime(double totalChargeAmount, double chargeRatePerSecond, int timeIntervalInSeconds)
        {
            if (totalChargeAmount < 0)
            {
                throw new ArgumentException("Charge amount cannot be negative.");
            }

            double chargeIncrement = chargeRatePerSecond * timeIntervalInSeconds;
            double chargeToAdd = 0;
            string status = "";

            while (chargeToAdd < totalChargeAmount && CurrentCharge < 100)
            {
                await Task.Delay(timeIntervalInSeconds * 1000); // Wait for the specified interval
                chargeToAdd += chargeIncrement;
                Charge(chargeIncrement); // Add the incremental charge

                status = $"Charging... Current charge: {CurrentCharge}%";

                if (CurrentCharge >= 100)
                {
                    status = "EV is fully charged.";
                    break;
                }
            }

            if (CurrentCharge < 100)
            {
                status = "Charging completed.";
            }

            return status;
        }

        // Method to discharge the EV over time asynchronously
        public async Task<string> DischargeOverTime(double totalDischargeAmount, double dischargeRatePerSecond, int timeIntervalInSeconds)
        {
            if (totalDischargeAmount < 0)
            {
                throw new ArgumentException("Discharge amount cannot be negative.");
            }

            double dischargeIncrement = dischargeRatePerSecond * timeIntervalInSeconds;
            double dischargeToSubtract = 0;
            string status = "";

            while (dischargeToSubtract < totalDischargeAmount && CurrentCharge > 0)
            {
                await Task.Delay(timeIntervalInSeconds * 1000); // Wait for the specified interval
                dischargeToSubtract += dischargeIncrement;
                Discharge(dischargeIncrement); // Subtract the incremental discharge

                status = $"Discharging... Current charge: {CurrentCharge}%";

                if (CurrentCharge <= 0)
                {
                    status = "EV is fully discharged.";
                    break;
                }
            }

            if (CurrentCharge > 0)
            {
                status = "Discharging completed.";
            }

            return status;
        }

        // Method to drive the EV a certain distance
        public string Drive(double distanceInKm)
        {
            if (distanceInKm < 0)
            {
                return "Distance cannot be negative.";
            }

            double totalConsumption = distanceInKm * consumptionRatePerMile;

            if (CurrentCharge < totalConsumption)
            {
                double possibleDistance = CurrentCharge / consumptionRatePerMile;
                CurrentCharge = 0;
                return $"You ran out of charge after driving {possibleDistance:F2} km.";
            }

            Discharge(totalConsumption);
            return $"You drove {distanceInKm:F2} km. Remaining charge: {CurrentCharge:F2}%.";
        }

        // Optional: Method to return the current charge
        public string GetCurrentCharge()
        {
            return CurrentCharge.ToString();
        }


    }
}
