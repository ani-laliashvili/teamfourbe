using CoreLibrary;
using EVOptimizationAPI.Dtos;
using EVOptimizationAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EVOptimizationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EVController : ControllerBase
    {
        private readonly IEVService _evService;

        public EVController(IEVService evService)
        {
            _evService = evService;
        }

        [HttpGet("{id}")]
        public ActionResult<EV> GetEV(int id)
        {
            try
            {
                var ev = _evService.GetEVById(id);
                var isRunningEssentialAppliances = _evService.IsRunningEssentialAppliances(id);
                var isRunningAllAppliances = _evService.IsRunningAllAppliances(id);

                var evStatus = new EVStatusDto
                {
                    CurrentCharge = ev.GetCurrentChargeInPercentage(),
                    IsRunningEssentialAppliances = isRunningEssentialAppliances,
                    IsRunningAllAppliances = isRunningAllAppliances
                };

                return Ok(evStatus);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: api/ev/charge
        [HttpPost("charge/{id}")]
        public IActionResult ChargeEV(int id, [FromBody] double amount)
        {
            _evService.ChargeEV(id, amount);
            return Ok($"EV {id} charged by {amount}kWh. Current charge: {_evService.GetEVById(id).GetCurrentChargeInPercentage()}%");
        }

        // POST: api/ev/run-essential-appliances
        [HttpPost("run-essential-appliances/{id}")]
        public IActionResult RunEssentialAppliances(int id, [FromBody] double amount)
        {
            _evService.RunEssentialAppliances(id, amount);
            return Ok($"Running essential appliances for EV {id}. Current charge: {_evService.GetEVById(id).GetCurrentChargeInPercentage()}%");
        }

        // POST: api/ev/run-all-appliances
        [HttpPost("run-all-appliances/{id}")]
        public IActionResult RunAllAppliances(int id, [FromBody] double amount)
        {
            _evService.RunAllAppliances(id, amount);
            return Ok($"Running all appliances for EV {id}. Current charge: {_evService.GetEVById(id).GetCurrentChargeInPercentage()}%");
        }

        // POST: api/ev/stop-appliances
        [HttpPost("stop-appliances/{id}")]
        public IActionResult StopAppliances(int id)
        {
            _evService.StopRunningAppliances(id);
            return Ok($"Appliances stopped for EV {id}.");
        }

        // POST: api/ev/stop
        [HttpPost("stop/{id}")]
        public IActionResult StopAction(int id)
        {
            _evService.StopCurrentOperation(id);
            return Ok($"Current operation stopped for EV {id}.");
        }

        // POST: api/ev/chargeovertime
        [HttpPost("chargeovertime/{id}")]
        public async Task<IActionResult> ChargeOverTime(int id, [FromBody] ChargeOverTimeDto request)
        {
            var result = await _evService.ChargeOverTime(id, request.ChargerPowerKWh, request.TimeIntervalHours);
            return Ok(result);
        }
    }
}
