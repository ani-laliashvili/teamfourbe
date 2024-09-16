namespace EVOptimizationAPI.Dtos
{
    public class DischargeOverTimeDto
    {
        public double TotalDischargeAmount { get; set; }
        public double DischargeRatePerSecond { get; set; }
        public int TimeIntervalInSeconds { get; set; }
    }
}
