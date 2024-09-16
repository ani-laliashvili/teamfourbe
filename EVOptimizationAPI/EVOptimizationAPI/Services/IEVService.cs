using CoreLibrary;
using System.Threading.Tasks;

namespace EVOptimizationAPI.Services
{
    public interface IEVService
    {
        EV GetEVById(int id); // Retrieves the EV by its unique ID
        void ChargeEV(int id, double amount); // Charges the EV by a specified amount
        void RunEssentialAppliances(int id, double amount); // Runs essential appliances for a specific EV
        void RunAllAppliances(int id, double amount); // Runs all appliances for a specific EV
        void StopRunningAppliances(int id); // Stops running appliances for a specific EV
        void AddEV(EV ev); // Adds a new EV to the system
        void StopCurrentOperation(int id); // Stops the current operation (charging or running appliances) for a specific EV
        Task<string> ChargeOverTime(int id, double ChargerPowerKWh, double timeIntervalInHours); // Charges the EV over time asynchronously
        bool IsRunningEssentialAppliances(int id); // Checks if essential appliances are running
        bool IsRunningAllAppliances(int id); // Checks if all appliances are running
    }
}
