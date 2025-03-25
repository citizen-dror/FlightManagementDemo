using FlightManagement.Common.DTOs;
using FlightManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlightManagement.AlertService.Controllers
{
    [ApiController]
    [Route("api/alerts")]
    public class PriceAlertsController : ControllerBase
    {
        private readonly IPriceAlertService _priceAlertService;

        public PriceAlertsController(IPriceAlertService priceAlertService)
        {
            _priceAlertService = priceAlertService;
        }

        // GET: api/alerts/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAlertsByUser(Guid userId)
        {
            var alerts = await _priceAlertService.GetAlertsByUserIdAsync(userId);
            if (alerts == null)
                return NotFound($"No alerts found for user with ID {userId}");

            return Ok(alerts);
        }

        // POST: api/alerts
        [HttpPost]
        public async Task<IActionResult> CreateAlert([FromBody] PriceAlertDto alertDto)
        {
            if (alertDto == null)
                return BadRequest("Alert data is required.");

            var createdAlert = await _priceAlertService.CreateAlertAsync(alertDto);
            return CreatedAtAction(nameof(GetAlertsByUser), new { userId = createdAlert.UserId }, createdAlert);
        }

        // DELETE: api/alerts/{alertId}
        [HttpDelete("{alertId}")]
        public async Task<IActionResult> DeleteAlert(Guid alertId)
        {
            var result = await _priceAlertService.DeleteAlertAsync(alertId);
            if (!result)
                return NotFound($"Alert with ID {alertId} not found.");

            return NoContent();
        }
    }
}
