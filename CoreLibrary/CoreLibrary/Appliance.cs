

namespace CoreLibrary
{
    public class Appliance
    {
        public int Id { get; set; }
        public string Name { get; private set; }
        public double PowerConsumption { get; set; }
        public bool IsRunning { get; private set; }

        // Constructor to initialize the component with a name and charge needed to run
        public Appliance(int id, string name, double powerConsumption, bool isRunning)
        {
            Name = name;
            Id = id;
            PowerConsumption = powerConsumption;
            IsRunning = isRunning;
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
        // Method to receive charge from the EV
        /*public string ReceiveCharge(double amount)
        {
            if (amount < 0)
            {
                return "Charge amount cannot be negative.";
            }

            ChargeReceived += amount;
            return $"Component {Name} received {amount}% charge. Total charge received: {ChargeReceived}%.";
        }

        // Method to check if the component has enough charge to run
        public string CheckIfCanRun()
        {
            if (ChargeReceived >= ChargeNeededToRun)
            {
                IsRunning = true;
                return $"Component {Name} is now running.";
            }
            else
            {
                IsRunning = false;
                return $"Component {Name} cannot run. It needs {ChargeNeededToRun - ChargeReceived:F2}% more charge to start.";
            }
        }*/

        /*// Method to simulate the component using charge while running
        public string UseChargeWhileRunning(double amount)
        {
            if (!IsRunning)
            {
                return $"Component {Name} is not running.";
            }

            if (amount < 0)
            {
                return "Usage amount cannot be negative.";
            }

            if (ChargeReceived >= amount)
            {
                ChargeReceived -= amount;
                return $"Component {Name} used {amount}% charge. Remaining charge: {ChargeReceived:F2}%.";
            }
            else
            {
                IsRunning = false;
                double usedCharge = ChargeReceived;
                ChargeReceived = 0;
                return $"Component {Name} ran out of charge after using {usedCharge:F2}%. Component is now stopped.";
            }
        }*/

        // Method to stop the component
        public string StopComponent()
        {
            if (IsRunning)
            {
                IsRunning = false;
                return $"Component {Name} has stopped.";
            }
            else
            {
                return $"Component {Name} is already stopped.";
            }
        }

        /*// Method to simulate running over time asynchronously
        public async Task<string> RunOverTime(double consumptionRatePerSecond, int timeIntervalInSeconds, double totalTimeInSeconds)
        {
            if (consumptionRatePerSecond < 0)
            {
                throw new ArgumentException("Consumption rate cannot be negative.");
            }

            if (!IsRunning)
            {
                return $"Component {Name} is not running.";
            }

            double totalConsumption = 0;
            string status = "";

            for (double elapsedTime = 0; elapsedTime < totalTimeInSeconds; elapsedTime += timeIntervalInSeconds)
            {
                await Task.Delay(timeIntervalInSeconds * 1000); // Wait for the specified interval

                double consumptionForInterval = consumptionRatePerSecond * timeIntervalInSeconds;
                totalConsumption += consumptionForInterval;

                status = UseChargeWhileRunning(consumptionForInterval); // Simulate usage

                if (!IsRunning)
                {
                    status = $"Component {Name} ran out of charge after running for {elapsedTime:F2} seconds.";
                    break;
                }

                status = $"Component {Name} is running. Time elapsed: {elapsedTime + timeIntervalInSeconds:F2} seconds. Remaining charge: {ChargeReceived:F2}%.";
            }

            if (IsRunning)
            {
                status = $"Component {Name} finished running for {totalTimeInSeconds:F2} seconds. Remaining charge: {ChargeReceived:F2}%.";
            }

            return status;
        }*/
    }    
}
