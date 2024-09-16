using System.Collections.Generic;

namespace EVOptimization
{
    public class Household
    {
        public int Id { get; set; }
        public List<int> EVs { get; set; } = new List<int>();
        public List<int> Appliances { get; set; } = new List<int>();
        public List<int> EssentialAppliances { get; set; } = new List<int>();
        public bool AcceptsRecommendations { get; set; }

        // Function to create sample households
        public static List<Household> CreateHouseholds()
        {
            List<Household> households = new List<Household>();

            // Create two households
            households.Add(new Household
            {
                Id = 1,
                EVs = new List<int> { 1 },
                Appliances = new List<int> { 1, 2, 3 },
                EssentialAppliances = new List<int> { 1 }, // E.g., fridge
                AcceptsRecommendations = true
            });

            households.Add(new Household
            {
                Id = 2,
                EVs = new List<int> { 2 },
                Appliances = new List<int> { 1, 2, 3 },
                EssentialAppliances = new List<int> { 1, 2 }, // E.g., fridge, lights
                AcceptsRecommendations = false
            });

            return households;
        }
    }
}