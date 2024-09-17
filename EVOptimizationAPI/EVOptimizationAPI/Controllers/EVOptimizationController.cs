using EVOptimizationAPI.Dtos;
using EVOptimizationAPI.Services;
using Microsoft.AspNetCore.Mvc;
using EVOptimization;
using Google.OrTools.LinearSolver;
using CoreLibrary;
using System.Collections.Generic;

namespace EVOptimizationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EVOptimizationController : ControllerBase
    {
        private readonly IEVOptimizationService _evOptimizationService;

        public EVOptimizationController(IEVOptimizationService evOptimizationService)
        {
            _evOptimizationService = evOptimizationService;
        }

        [HttpGet("optimize")]
        public ActionResult<OptimizationResultDto> OptimizeEVCharging()
        {
            Solver solver = Solver.CreateSolver("CBC_MIXED_INTEGER_PROGRAMMING");
            InitializationData data = Initialize();
            var results = Optimization.SolveOptimization(solver, data.NumTimeSlots, data.Households, data.EVs, data.Appliances, data.P_Price, data.Outage);

            var optimizationResults = new List<OptimizationResultDto>();

            // Iterate through each EVResult in the results
            foreach (var evResult in results.EVResults)
            {
                // Get the StateOfCharge list for the current EV
                var stateOfChargeList = evResult.GetStateOfChargeList();

                // Get the combined power series for the current EV
                var combinedPowerSeries = evResult.GetCombinedPowerSeries();

                // Create a result DTO for this EV
                var result = new OptimizationResultDto
                {
                    EVId = evResult.EVId, // Use the actual EV ID
                    ChargeLevelsPer60Min = stateOfChargeList, // Charge level per hour
                    ChargingSchedule = combinedPowerSeries, // Charging schedule per hour (combined charge and discharge)
                    FinalCharge = stateOfChargeList.Last() // Final charge after all hours
                };

                // Add this result to the list of results
                optimizationResults.Add(result);
            }

            // Return the list of results
            return Ok(optimizationResults);


        }

        // Initialization of the data
        public static InitializationData Initialize()
        {
            // Time slots (24-hour period)
            int numTimeSlots = 24;

            // Create sample data for households, EVs, and appliances
            List<Household> households = Household.CreateHouseholds();
            List<EV> EVs = new()
            {
                new(id: 1, householdId: 1, batteryCapacity:50, initialCharge: GenerateNormalDistribution(60, 30) * 0.01, chargeEmergencyLevel: 0.6, isAvailableForDischarge:true),
                new(id: 2, householdId: 2, 30, GenerateNormalDistribution(60, 30) * 0.01, 0.75, true)
            };

            List<Appliance> appliances = new()
            {
                new(id: 1, name: "Fridge", powerConsumption: 0.2, isRunning: true), // kW
                new(2, "Lights", 0.1, true), // kW
                new(3, "HVAC", 2.0, false)    // kW
            };

            Outage outage = new(4, 5, "");

            // Electricity price forecast (prices in $ per kWh)
            double[] P_price = PriceForecast.CreatePriceForecast(numTimeSlots);

            // Return all initialized data
            return new()
            {
                NumTimeSlots = numTimeSlots,
                Households = households,
                EVs = EVs,
                Appliances = appliances,
                P_Price = P_price,
                Outage = outage
            };
        }
        
        // Method to generate a random number from a normal (Gaussian) distribution
        public static double GenerateNormalDistribution(double mean, double stddev)
        { 
            Random random = new Random();
            double u1 = 1.0 - random.NextDouble(); // uniform(0,1] random doubles
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // random normal(0,1)
            double randNormal = mean + stddev * randStdNormal; // random normal(mean,stdDev)
            return randNormal;
        }
        

        [HttpGet("mock-optimization")]
        public ActionResult<OptimizationResultDto> GetMockOptimization()
        {
            // Example projected charge levels, charging/discharging schedules for 24 hours (every hour)
            var chargeLevelsPerHour = new List<double>
            {
                50, 52, 54, 53, 50, 48, 45, 47, 49, 51, 52, 53,
                55, 57, 55, 53, 51, 49, 50, 52, 53, 54, 55, 57
            };

            // A single charging schedule with positive (charging) and negative (discharging) values
            var chargingSchedule = new List<double>
            {
                2, 2, -1, -3, -2, -3, 2, 2, 2, 1, 1, 2,
                2, 2, -2, -2, -2, -2, 2, 2, 1, 1, 1, 2
            };

            var result = new OptimizationResultDto
            {
                EVId = 1, // Example EV ID
                ChargeLevelsPer60Min = chargeLevelsPerHour, // Charge level per hour
                ChargingSchedule = chargingSchedule, // Charging schedule per hour
                FinalCharge = chargeLevelsPerHour.Last() // Final charge after 24 hours
            };

            return Ok(result);
        }
    }
}
