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
        private readonly Dictionary<int, bool> _isDischarging = new Dictionary<int, bool>();
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
            _isCharging[_nextId] = false; // Initialize charging and discharging status
            _isDischarging[_nextId] = false;
            _nextId++;           // Increment the ID for the next EV
        }

        // Method to charge the EV by a specified amount
        public void ChargeEV(int id, double amount)
        {
            var ev = GetEVById(id);
            ev.Charge(amount);
        }

        // Method to discharge the EV by a specified amount
        public void DischargeEV(int id, double amount)
        {
            var ev = GetEVById(id);
            ev.Discharge(amount);
        }

        // Method to stop charging or discharging for an EV
        public void StopCurrentOperation(int id)
        {
            if (!_evs.ContainsKey(id)) throw new KeyNotFoundException("EV not found.");

            if (_isCharging[id])
            {
                _isCharging[id] = false;
                System.Console.WriteLine($"Charging operation for EV {id} stopped.");
            }

            if (_isDischarging[id])
            {
                _isDischarging[id] = false;
                System.Console.WriteLine($"Discharging operation for EV {id} stopped.");
            }
        }

        // Method to charge the EV over time asynchronously
        public async Task<string> ChargeOverTime(int id, double totalChargeAmount, double chargeRatePerSecond, int timeIntervalInSeconds)
        {
            var ev = GetEVById(id);
            _isCharging[id] = true;
            double chargeIncrement = chargeRatePerSecond * timeIntervalInSeconds;
            double chargeToAdd = 0;
            string status = "";

            while (chargeToAdd < totalChargeAmount && ev.CurrentCharge < 100 && _isCharging[id])
            {
                await Task.Delay(timeIntervalInSeconds * 1000); // Wait for the specified interval
                chargeToAdd += chargeIncrement;
                ev.Charge(chargeIncrement); // Add the incremental charge

                status = $"Charging EV {id}... Current charge: {ev.CurrentCharge}%";

                if (ev.CurrentCharge >= 100)
                {
                    status = $"EV {id} is fully charged.";
                    break;
                }
            }

            _isCharging[id] = false; // Charging complete or stopped

            if (ev.CurrentCharge < 100 && !_isCharging[id])
            {
                status = $"Charging stopped for EV {id}.";
            }

            return status;
        }

        // Method to discharge the EV over time asynchronously
        public async Task<string> DischargeOverTime(int id, double totalDischargeAmount, double dischargeRatePerSecond, int timeIntervalInSeconds)
        {
            var ev = GetEVById(id);
            _isDischarging[id] = true;
            double dischargeIncrement = dischargeRatePerSecond * timeIntervalInSeconds;
            double dischargeToSubtract = 0;
            string status = "";

            while (dischargeToSubtract < totalDischargeAmount && ev.CurrentCharge > 0 && _isDischarging[id])
            {
                await Task.Delay(timeIntervalInSeconds * 1000); // Wait for the specified interval
                dischargeToSubtract += dischargeIncrement;
                ev.Discharge(dischargeIncrement); // Subtract the incremental discharge

                status = $"Discharging EV {id}... Current charge: {ev.CurrentCharge}%";

                if (ev.CurrentCharge <= 0)
                {
                    status = $"EV {id} is fully discharged.";
                    break;
                }
            }

            _isDischarging[id] = false; // Discharging complete or stopped

            if (ev.CurrentCharge > 0 && !_isDischarging[id])
            {
                status = $"Discharging stopped for EV {id}.";
            }

            return status;
        }
    }
}
