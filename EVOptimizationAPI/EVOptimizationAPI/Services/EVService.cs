using CoreLibrary;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVOptimizationAPI.Services
{
    public class EVService : IEVService
    {
        // A simple in-memory dictionary to store EV objects, using an auto-incrementing ID
        private readonly Dictionary<int, EV> _evs = new Dictionary<int, EV>();
        private readonly Dictionary<int, bool> _isCharging = new Dictionary<int, bool>();
        private int _nextId = 1;

        public EV GetEVById(int id)
        {
            if (_evs.ContainsKey(id))
            {
                return _evs[id];
            }
            throw new KeyNotFoundException("EV not found.");
        }

        // AddEV method to add a new EV to the system
        public void AddEV(EV ev)
        {
            _evs[_nextId] = ev;  // Add the EV object to the dictionary
            _isCharging[_nextId] = false; // Initialize charging status
            _nextId++;           // Increment the ID for the next EV
        }

        // Method to charge the EV by a specified amount
        public void ChargeEV(int id, double amount)
        {
            var ev = GetEVById(id);
            ev.IsCharging = true;
            ev.Charge(amount);
        }

        // Method to run essential appliances for an EV
        public void RunEssentialAppliances(int id, double amount)
        {
            var ev = GetEVById(id);
            ev.RunEssentialAppliances(amount);
        }

        // Method to run all appliances for an EV
        public void RunAllAppliances(int id, double amount)
        {
            var ev = GetEVById(id);
            ev.RunAllAppliances(amount);
        }

        // Method to stop running appliances for an EV
        public void StopRunningAppliances(int id)
        {
            var ev = GetEVById(id);
            ev.StopRunningAppliances();
        }

        // Method to stop charging or running appliances for an EV
        public void StopCurrentOperation(int id)
        {
            if (!_evs.ContainsKey(id)) throw new KeyNotFoundException("EV not found.");

            if (_isCharging[id])
            {
                _isCharging[id] = false;
                System.Console.WriteLine($"Charging operation for EV {id} stopped.");
            }

            var ev = GetEVById(id);
            ev.StopRunningAppliances(); // Stop any appliance usage
            ev.IsCharging=false;
        }

        // Background method to charge the EV over time
        public void ChargeOverTime(int id, double ChargerPowerKWh, double timeIntervalInHours, double? chargeUntil = null)
        {
            // Set isCharging to true immediately before running the background task
            var ev = GetEVById(id);
            ev.IsCharging = true;

            Task.Run(() =>
            {
                if (timeIntervalInHours <= 0)
                    return;

                // Set default `chargeUntil` to EV's BatteryCapacity if not specified
                if (chargeUntil == null || chargeUntil > ev.BatteryCapacity)
                    chargeUntil = ev.BatteryCapacity;

                // Charge increment is the power provided (in kWh) multiplied by the time interval
                double chargeIncrement = ChargerPowerKWh * timeIntervalInHours;

                while (ev.CurrentCharge < chargeUntil && ev.CurrentCharge < ev.BatteryCapacity && ev.IsCharging)
                {
                    // Block the current thread for the specified time interval
                    System.Threading.Thread.Sleep((int)(timeIntervalInHours * 3600 * 1000)); // Convert hours to milliseconds

                    // Add the charge increment, but ensure it doesn't exceed the target charge or battery capacity
                    ev.Charge(Math.Min(chargeIncrement, chargeUntil.Value - ev.CurrentCharge));

                    // Check if the EV is fully charged or reached the target charge
                    if (ev.CurrentCharge >= ev.BatteryCapacity || ev.CurrentCharge >= chargeUntil)
                    {
                        break;
                    }
                }

                ev.IsCharging = false; // Charging complete or stopped
            });
        }



        // Method to check if essential appliances are running
        public bool IsRunningEssentialAppliances(int id)
        {
            var ev = GetEVById(id);
            return ev.IsRunningEssentialAppliances;
        }

        // Method to check if all appliances are running
        public bool IsRunningAllAppliances(int id)
        {
            var ev = GetEVById(id);
            return ev.IsRunningAllAppliances;
        }
    }
}
