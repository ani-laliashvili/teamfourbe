namespace EVOptimizationAPI.Dtos
{
    public class ChargeOverTimeDto
    {
        public double ChargerPowerKWh { get; set; } 
        public double TimeIntervalHours { get; set; }
        public double? ChargeUntil { get; set; } = null;
    }
}
