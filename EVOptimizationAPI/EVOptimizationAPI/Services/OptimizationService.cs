using EVOptimizationAPI.Dtos;

namespace EVOptimizationAPI.Services
{
    public class EVOptimizationService : IEVOptimizationService
    {
        private readonly IEVService _evService;

        public EVOptimizationService(IEVService evService)
        {
            _evService = evService;
        }

        public OptimizationResultDto OptimizeCharging(OptimizationInputDto input)
        {
            var ev = _evService.GetEVById(input.EVId);
            double projectedCharge = ev.CurrentCharge;
            var chargingSchedule = new List<double>();
            var dischargingSchedule = new List<double>();
            var chargeLevels = new List<double>();



            // Simulate the optimization over 24 hours
            for (int i = 0; i < 24; i++)
            {
                double usage = input.ForecastedUsagePer60Min[i];

                if (projectedCharge < input.DesiredFinalCharge && input.MaxChargingPower > 0)
                {
                    // Charge the EV if it has room and needs to reach the desired final charge
                    double chargeAmount = Math.Min(input.MaxChargingPower, input.DesiredFinalCharge - projectedCharge);
                    projectedCharge += chargeAmount;
                    chargingSchedule.Add(chargeAmount);
                    dischargingSchedule.Add(0);
                }
                else if (usage > 0 && input.MaxDischargingPower > 0 && projectedCharge > 0)
                {
                    // Discharge if there's forecasted usage and the EV has charge
                    double dischargeAmount = Math.Min(input.MaxDischargingPower, projectedCharge);
                    projectedCharge -= dischargeAmount;
                    chargingSchedule.Add(0);
                    dischargingSchedule.Add(dischargeAmount);
                }
                else
                {
                    chargingSchedule.Add(0);
                    dischargingSchedule.Add(0);
                }

                // Apply the forecasted usage
                projectedCharge = Math.Max(0, projectedCharge - usage);

                // Save projected charge level for this interval
                chargeLevels.Add(projectedCharge);
            }

            return new OptimizationResultDto
            {
                EVId = input.EVId,
                ChargeLevelsPer60Min = chargeLevels,
                ChargingSchedule = chargingSchedule,
                FinalCharge = projectedCharge
            };
        }
    }
}
