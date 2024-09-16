using CoreLibrary;
using System.Collections.Generic;

namespace EVOptimizationAPI.Services
{
    public class EVService : IEVService
    {
        // A simple in-memory dictionary to store EV objects, using an auto-incrementing ID
        private readonly Dictionary<int, EV> _evs = new Dictionary<int, EV>();
        private int _nextId = 1;

        public EV GetEVById(int id)
        {
            if (_evs.ContainsKey(id))
            {
                return _evs[id];
            }
            throw new KeyNotFoundException("EV not found.");
        }

        public void ChargeEV(int id, double amount)
        {
            var ev = GetEVById(id);
            ev.Charge(amount);
        }

        public void DischargeEV(int id, double amount)
        {
            var ev = GetEVById(id);
            ev.Discharge(amount);
        }

        // AddEV method to add a new EV to the system
        public void AddEV(EV ev)
        {
            _evs[_nextId] = ev;  // Add the EV object to the dictionary
            _nextId++;           // Increment the ID for the next EV
        }
    }
}