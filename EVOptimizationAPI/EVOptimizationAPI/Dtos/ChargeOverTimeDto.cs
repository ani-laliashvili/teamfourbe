namespace EVOptimizationAPI.Dtos
{
    public class ChargeOverTimeDto
    {
        public double TotalChargeAmount { get; set; }
        public double ChargeRatePerSecond { get; set; }
        public int TimeIntervalInSeconds { get; set; }
    }
}
