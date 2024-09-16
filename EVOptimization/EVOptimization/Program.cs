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

        // Class to store initialization data
        public class InitializationData
        {
            public int NumTimeSlots { get; set; }
            public List<Household> Households { get; set; }
            public List<EV> EVs { get; set; }
            public List<Appliance> Appliances { get; set; }
            public double[] P_Price { get; set; }
            public Outage Outage { get; set; }
        }

        // Initialization of the data
        public static InitializationData Initialize()
        {
            // Time slots (24-hour period)
            int numTimeSlots = 24;

            // Create sample data for households, EVs, and appliances
            List<Household> households = Household.CreateHouseholds();
            List<EV> EVs = new List<EV>
            {
                new EV(1, 1, 50, 60, 0.8, true),  
                new EV(2, 2, 30, 70, 0.75, true)   
            };

            List<Appliance> appliances = Appliance.CreateAppliances();
            Outage outage = new Outage(4, 5); 

            // Electricity price forecast (prices in $ per kWh)
            double[] P_price = PriceForecast.CreatePriceForecast(numTimeSlots);

            // Return all initialized data
            return new InitializationData
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
