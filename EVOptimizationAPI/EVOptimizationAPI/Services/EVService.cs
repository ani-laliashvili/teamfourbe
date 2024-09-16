using CoreLibrary;

namespace EVOptimizationAPI.Services
{
    public class EVService : IEVService
    {
        private readonly Dictionary<int, EV> _evs = new Dictionary<int, EV>();

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
    }
}
