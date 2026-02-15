
using ContentHook.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace ContentHook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranscriptsController : ControllerBase
    {
        private readonly ITranscriptService _service;

        public TranscriptsController(ITranscriptService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string userId, string text, string? language)
        {
            var transcript = await _service.CreateAsync(userId, text, language);
            return Ok(transcript);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var transcript = await _service.GetByIdAsync(id);
            if (transcript == null)
                return NotFound();

            return Ok(transcript);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var transcripts = await _service.GetAllAsync();
            return Ok(transcripts);
        }
    }
}
