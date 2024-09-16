using CoreLibrary;
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
    }
}
