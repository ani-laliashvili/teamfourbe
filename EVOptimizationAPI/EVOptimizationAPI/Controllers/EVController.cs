using CoreLibrary;
using EVOptimizationAPI.Dtos;
using EVOptimizationAPI.Services;
using Microsoft.AspNetCore.Mvc;

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
                return Ok(_evService.GetEVById(id));
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
            return Ok($"EV {id} charged by {amount}%. Current charge: {_evService.GetEVById(id).GetCurrentCharge()}%");
        }

        // POST: api/ev/discharge
        [HttpPost("discharge/{id}")]
        public IActionResult DischargeEV(int id, [FromBody] double amount)
        {
            _evService.DischargeEV(id, amount);
            return Ok($"EV {id} discharged by {amount}%. Current charge: {_evService.GetEVById(id).GetCurrentCharge()}%");
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
            var result = await _evService.ChargeOverTime(id, request.TotalChargeAmount, request.ChargeRatePerSecond, request.TimeIntervalInSeconds);
            return Ok(result);
        }

        // POST: api/ev/dischargeovertime
        [HttpPost("dischargeovertime/{id}")]
        public async Task<IActionResult> DischargeOverTime(int id, [FromBody] DischargeOverTimeDto request)
        {
            var result = await _evService.DischargeOverTime(id, request.TotalDischargeAmount, request.DischargeRatePerSecond, request.TimeIntervalInSeconds);
            return Ok(result);
        }
    }
}
