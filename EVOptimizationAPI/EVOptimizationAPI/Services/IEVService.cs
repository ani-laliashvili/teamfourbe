using CoreLibrary;

namespace EVOptimizationAPI.Services
{
    public interface IEVService
    {
        EV GetEVById(int id);
        void ChargeEV(int id, double amount);
        void DischargeEV(int id, double amount);
        void AddEV(EV ev); // AddEV method to add a new EV
    }
}
