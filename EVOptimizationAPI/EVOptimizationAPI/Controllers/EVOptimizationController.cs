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
            // Example projected charge levels, charging and discharging schedules for 24 hours (every hour)
            var chargeLevelsPerHour = new List<double>
        {
            50, 52, 54, 56, 58, 60, 65, 70, 75, 80, 82, 84,
            86, 88, 89, 90, 91, 92, 94, 95, 96, 97, 98, 100
        };

            var chargingSchedule = new List<double>
        {
            2, 2, 2, 2, 2, 2, 5, 5, 5, 5, 2, 2,
            2, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2
        };

            var dischargingSchedule = new List<double>
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

            var result = new OptimizationResultDto
            {
                EVId = 1, // Example EV ID
                ChargeLevelsPer60Min = chargeLevelsPerHour, // Charge level per hour
                ChargingSchedule = chargingSchedule, // Charging schedule per hour
                DischargingSchedule = dischargingSchedule, // Discharging schedule per hour
                FinalCharge = chargeLevelsPerHour.Last() // Final charge after 24 hours
            };

            return Ok(result);
        }
    }
}
