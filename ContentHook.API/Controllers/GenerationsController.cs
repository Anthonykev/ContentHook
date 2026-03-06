using ContentHook.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContentHook.API.DTOs;
using System.Security.Claims;

namespace ContentHook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GenerationsController : ControllerBase
    {
        private readonly IGenerationRepository _repo;

        public GenerationsController(IGenerationRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var generation = await _repo.GetByIdForUserAsync(id, userId);
            if (generation is null) return NotFound();

            return Ok(new GenerationResponseDto(
                 generation.Id, generation.Platform, generation.Title,
                 generation.Hook, generation.Hashtags, generation.ModelUsed,
                 generation.PromptVersion, generation.RegenerationIndex, generation.CreatedAt
));
        }

        [HttpGet("by-transcript/{transcriptId:guid}")]
        public async Task<IActionResult> GetByTranscript(Guid transcriptId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // ← NEU
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var generations = await _repo.GetByTranscriptIdForUserAsync(transcriptId, userId);
            return Ok(generations.Select(g => new GenerationResponseDto(
                g.Id, g.Platform, g.Title, g.Hook, g.Hashtags,
                g.ModelUsed, g.PromptVersion, g.RegenerationIndex, g.CreatedAt
            )));
        }
    }
}