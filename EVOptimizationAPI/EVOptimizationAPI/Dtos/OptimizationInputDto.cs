namespace EVOptimizationAPI.Dtos
{
    public class OptimizationInputDto
    {
        public int EVId { get; set; } // The ID of the EV being optimized
        public double DesiredFinalCharge { get; set; } // Desired charge at the end of 24 hours
        public double MaxChargingPower { get; set; } // Max charging power in kW
        public double MaxDischargingPower { get; set; } // Max discharging power in kW
        public List<double> ForecastedUsagePer60Min { get; set; } // Forecasted power consumption for each 5-minute interval
    }
}
