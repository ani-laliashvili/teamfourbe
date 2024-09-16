using Google.OrTools.LinearSolver;
using CoreLibrary;

namespace EVOptimization
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Initialize the solver
            Solver solver = Solver.CreateSolver("CBC_MIXED_INTEGER_PROGRAMMING");
            
            try
            {
                // Initialize data
                InitializationData data = Initialize();

                // Call a separate method to set up and solve the optimization model
                var results = Optimization.SolveOptimization(solver, data.NumTimeSlots, data.Households, data.EVs, data.Appliances, data.P_Price, data.Outage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
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
                new(id: 1, householdId: 1, batteryCapacity:50, initialCharge:60 * 0.01, chargeEmergencyLevel: 0.6, isAvailableForDischarge:true),  
                new(id: 2, householdId: 2, 30, 70 * 0.01, 0.75, true)   
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
    }
}
