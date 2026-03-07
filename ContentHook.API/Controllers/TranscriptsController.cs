using ContentHook.API.DTOs;
using ContentHook.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTranscriptRequest request)
        {
            // TODO: replace with User.FindFirst("sub")?.Value
            const string placeholderUserId = "anonymous";

            var transcript = await _service.CreateAsync(
                placeholderUserId,
                request.Text,
                request.Language,
                originalFileName: null  
            );

            return CreatedAtAction(
                nameof(GetById),
                new { id = transcript.Id },
                transcript
            );
        }

  
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var transcript = await _service.GetByIdAsync(id);
            if (transcript is null)
                return NotFound();

            return Ok(transcript);
        }

      
        /// TODO:  Filter by authenticated UserId.
       
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var transcripts = await _service.GetAllAsync();
            return Ok(transcripts);
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateText(Guid id, [FromBody] UpdateTranscriptRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            try
            {
                var transcript = await _service.UpdateTextAsync(id, userId, request.Text);
                return Ok(new { transcript.Id, transcript.Text, transcript.UpdatedAt });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

    }
}