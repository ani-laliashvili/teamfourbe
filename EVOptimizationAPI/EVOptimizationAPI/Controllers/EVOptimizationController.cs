using EVOptimizationAPI.Dtos;
using EVOptimizationAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EVOptimizationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EVOptimizationController : ControllerBase
    {
        private readonly IEVOptimizationService _evOptimizationService;

        public EVOptimizationController(IEVOptimizationService evOptimizationService)
        {
            _evOptimizationService = evOptimizationService;
        }

        [HttpPost("optimize")]
        public ActionResult<OptimizationResultDto> OptimizeEVCharging([FromBody] OptimizationInputDto input)
        {
            var result = _evOptimizationService.OptimizeCharging(input);
            return Ok(result);
        }

        [HttpGet("mock-optimization")]
        public ActionResult<OptimizationResultDto> GetMockOptimization()
        {
            // Example projected charge levels, charging/discharging schedules for 24 hours (every hour)
            var chargeLevelsPerHour = new List<double>
            {
                50, 52, 54, 53, 50, 48, 45, 47, 49, 51, 52, 53,
                55, 57, 55, 53, 51, 49, 50, 52, 53, 54, 55, 57
            };

            // A single charging schedule with positive (charging) and negative (discharging) values
            var chargingSchedule = new List<double>
            {
                2, 2, -1, -3, -2, -3, 2, 2, 2, 1, 1, 2,
                2, 2, -2, -2, -2, -2, 2, 2, 1, 1, 1, 2
            };

            var result = new OptimizationResultDto
            {
                EVId = 1, // Example EV ID
                ChargeLevelsPer60Min = chargeLevelsPerHour, // Charge level per hour
                ChargingSchedule = chargingSchedule, // Charging schedule per hour
                FinalCharge = chargeLevelsPerHour.Last() // Final charge after 24 hours
            };

            return Ok(result);
        }
    }
}
