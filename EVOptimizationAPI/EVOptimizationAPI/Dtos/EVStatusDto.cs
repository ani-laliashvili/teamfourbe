namespace EVOptimizationAPI.Dtos
{
    public class EVStatusDto
    {
        public double CurrentCharge { get; set; }
        public bool IsRunningEssentialAppliances { get; set; }
        public bool IsRunningAllAppliances { get; set; }
        public bool IsCharging { get; set; }
    }
}
