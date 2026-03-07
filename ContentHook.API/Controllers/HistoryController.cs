using ContentHook.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ContentHook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HistoryController : ControllerBase
    {
        private readonly HistoryService _historyService;

        public HistoryController(HistoryService historyService)
        {
            _historyService = historyService;
        }

        // GET /api/history
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var history = await _historyService.GetAllByUserAsync(userId);
            return Ok(history);
        }

        // GET /api/history/{jobId}
        [HttpGet("{jobId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByJobId(Guid jobId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var item = await _historyService.GetByJobIdAsync(userId, jobId);
            if (item is null)
                return NotFound();

            return Ok(item);
        }
    }
}