namespace EVOptimizationAPI.Dtos
{
    public class OptimizationResultDto
    {
        public int EVId { get; set; }
        public List<double> ChargeLevelsPer60Min { get; set; } // Projected charge levels for each 60-minute interval
        public List<double> ChargingSchedule { get; set; } // Optimized charging schedule for each 60-minute interval
        public double FinalCharge { get; set; } // Final projected charge after 24 hours
    }

}
