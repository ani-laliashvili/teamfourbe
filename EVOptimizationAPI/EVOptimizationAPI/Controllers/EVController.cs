using CoreLibrary;
using EVOptimization;
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

        [HttpPost("{id}/charge")]
        public IActionResult ChargeEV(int id, [FromBody] double amount)
        {
            _evService.ChargeEV(id, amount);
            return Ok("EV charged.");
        }

        [HttpPost("{id}/discharge")]
        public IActionResult DischargeEV(int id, [FromBody] double amount)
        {
            _evService.DischargeEV(id, amount);
            return Ok("EV discharged.");
        }

        // POST: api/ev/stop
        [HttpPost("stop")]
        public IActionResult StopAction()
        {
            _evService.StopCurrentOperation();
            return Ok("Current operation stopped.");
        }

        // POST: api/ev/chargeovertime
        [HttpPost("chargeovertime")]
        public async Task<IActionResult> ChargeOverTime([FromBody] ChargeOverTimeDto request)
        {
            var result = await _ev.ChargeOverTime(request.TotalChargeAmount, request.ChargeRatePerSecond, request.TimeIntervalInSeconds);
            return Ok(result);
        }

        // POST: api/ev/dischargeovertime
        [HttpPost("dischargeovertime")]
        public async Task<IActionResult> DischargeOverTime([FromBody] DischargeOverTimeDto request)
        {
            var result = await _ev.DischargeOverTime(request.TotalDischargeAmount, request.DischargeRatePerSecond, request.TimeIntervalInSeconds);
            return Ok(result);
        }
    }
}
}
