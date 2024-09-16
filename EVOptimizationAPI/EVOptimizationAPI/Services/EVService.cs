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
        }

        // Method to charge the EV over time asynchronously
        public async Task<string> ChargeOverTime(int id, double ChargerPowerKWh, double timeIntervalInHours, double? chargeUntil = null)
        {
            if (timeIntervalInHours <= 0)
                return "Invalid time interval";

            var ev = GetEVById(id);

            // Set default `chargeUntil` to EV's BatteryCapacity if not specified
            if (chargeUntil == null || chargeUntil > ev.BatteryCapacity)
                chargeUntil = ev.BatteryCapacity;

            _isCharging[id] = true;

            // Charge increment is the power provided (in kWh) multiplied by the time interval
            double chargeIncrement = ChargerPowerKWh * timeIntervalInHours;
            string status = "";

            while (ev.CurrentCharge < chargeUntil && ev.CurrentCharge < ev.BatteryCapacity && _isCharging[id])
            {
                // Wait for the time interval before adding the next charge increment
                await Task.Delay((int)(timeIntervalInHours * 3600 * 1000)); // Convert hours to milliseconds

                // Add the charge increment, but ensure it doesn't exceed the target charge or battery capacity
                ev.Charge(Math.Min(chargeIncrement, chargeUntil.Value - ev.CurrentCharge));

                // Update status
                status = $"Charging EV {id}... Current charge: {ev.CurrentCharge} kWh";

                // Stop charging if the EV reaches the target charge or battery capacity
                if (ev.CurrentCharge >= ev.BatteryCapacity)
                {
                    status = $"EV {id} is fully charged at {ev.BatteryCapacity} kWh.";
                    break;
                }

                if (ev.CurrentCharge >= chargeUntil)
                {
                    status = $"EV {id} reached the target charge of {chargeUntil} kWh.";
                    break;
                }
            }

            _isCharging[id] = false; // Charging complete or stopped

            if (ev.CurrentCharge < ev.BatteryCapacity && !_isCharging[id])
            {
                status = $"Charging stopped for EV {id}.";
            }

            return status;
        }


        // Method to check if essential appliances are running
        public bool IsRunningEssentialAppliances(int id)
        {
            var ev = GetEVById(id);
            return ev.IsRunningEssentialAppliances();
        }

        // Method to check if all appliances are running
        public bool IsRunningAllAppliances(int id)
        {
            var ev = GetEVById(id);
            return ev.IsRunningAllAppliances();
        }
    }
}
