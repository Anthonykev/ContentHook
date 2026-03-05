using ContentHook.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContentHook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var generation = await _repo.GetByIdAsync(id);
            if (generation is null) return NotFound();

            return Ok(new
            {
                generation.Id,
                generation.Platform,
                generation.Title,
                generation.Hook,
                generation.Hashtags,
                generation.ModelUsed,
                generation.PromptVersion,
                generation.RegenerationIndex,
                generation.CreatedAt
            });
        }

        [HttpGet("by-transcript/{transcriptId:guid}")]
        public async Task<IActionResult> GetByTranscript(Guid transcriptId)
        {
            var generations = await _repo.GetByTranscriptIdAsync(transcriptId);
            return Ok(generations.Select(g => new
            {
                g.Id,
                g.Platform,
                g.Title,
                g.Hook,
                g.Hashtags,
                g.ModelUsed,
                g.PromptVersion,
                g.RegenerationIndex,
                g.CreatedAt
            }));
        }
    }
}