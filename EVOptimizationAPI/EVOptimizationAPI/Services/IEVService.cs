using CoreLibrary;
using System.Threading.Tasks;

namespace EVOptimizationAPI.Services
{
    public interface IEVService
    {
        EV GetEVById(int id); // Retrieves the EV by its unique ID
        void ChargeEV(int id, double amount); // Charges the EV by a specified amount
        void DischargeEV(int id, double amount); // Discharges the EV by a specified amount
        void AddEV(EV ev); // Adds a new EV to the system
        void StopCurrentOperation(int id); // Stops the current operation (charging or discharging) for a specific EV
        Task<string> ChargeOverTime(int id, double totalChargeAmount, double chargeRatePerSecond, int timeIntervalInSeconds); // Charges the EV over time asynchronously
        Task<string> DischargeOverTime(int id, double totalDischargeAmount, double dischargeRatePerSecond, int timeIntervalInSeconds); // Discharges the EV over time asynchronously
    }
}
