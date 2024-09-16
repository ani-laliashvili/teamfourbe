using System;

namespace CoreLibrary
{
    public class EV
    {
        // Current charge of the EV in percentage (0 to 100)
        public double CurrentCharge { get; private set; }
        private readonly double consumptionRatePerMile;

        // Flags to track if charging or discharging is active
        private bool isCharging;
        private bool isDischarging;

        // Constructor to initialize the current charge and consumption rate
        public EV(double initialCharge, double consumptionRatePerMile)
        {
            if (initialCharge < 0 || initialCharge > 100)
            {
                throw new ArgumentException("Initial charge must be between 0 and 100.");
            }

            if (consumptionRatePerMile <= 0)
            {
                throw new ArgumentException("Consumption rate must be positive.");
            }

            CurrentCharge = initialCharge;
            this.consumptionRatePerMile = consumptionRatePerMile;
            isCharging = false;
            isDischarging = false;
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

        // Override method to stop the current operation (charging or discharging)
        public void StopCurrentOperation()
        {
            if (isCharging)
            {
                isCharging = false;
                Console.WriteLine("Charging operation stopped.");
            }
            else if (isDischarging)
            {
                isDischarging = false;
                Console.WriteLine("Discharging operation stopped.");
            }
            else
            {
                Console.WriteLine("No operation to stop.");
            }
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
