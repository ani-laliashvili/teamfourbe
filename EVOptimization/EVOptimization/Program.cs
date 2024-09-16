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
            if (solver == null)
            {
                Console.WriteLine("Could not create solver.");
                return;
            }

            // Time slots (24-hour period)
            int numTimeSlots = 24;
            int[] timeSlots = new int[numTimeSlots];
            for (int h = 0; h<numTimeSlots; h++)
            {
                timeSlots[h] = h;
            }

            // Create sample data for households, EVs, and appliances
            List<Household> households = Household.CreateHouseholds();
            List<EV> EVs = EV.CreateEVs(households);
            List<Appliance> appliances = Appliance.CreateAppliances();

            Outage outage = new Outage();

            // Electricity price forecast (prices in $ per kWh)
            double[] P_price = PriceForecast.CreatePriceForecast(numTimeSlots);

            // Call a separate method to set up and solve the optimization model
            Optimization.SolveOptimization(solver, numTimeSlots, households, EVs, appliances, P_price, outage);
        }
    }
}
