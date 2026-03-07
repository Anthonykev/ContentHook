using ContentHook.API.DTOs;
using ContentHook.DAL.Entities;
using ContentHook.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ContentHook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingRepository _repo;

        public RatingsController(IRatingRepository repo) => _repo = repo;

        // GET /api/ratings/my — Has logged-in user already rated?
        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            return Ok(new { hasRated = await _repo.HasUserRatedAsync(userId) });
        }

        // DELETE /api/ratings/my — Delete own rating (re-rating)
        [HttpDelete("my")]
        public async Task<IActionResult> DeleteMy()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            await _repo.DeleteByUserIdAsync(userId);
            return NoContent();
        }

        // GET /api/ratings/summary — Category averages
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var ratings = await _repo.GetAllAsync();
            if (ratings.Count == 0)
                return Ok(new RatingSummaryDto(0, 0, 0, 0, 0, 0));

            double Avg(params Func<Evaluation, int>[] selectors)
                => selectors.SelectMany(s => ratings.Select(s)).Average();

            var summary = new RatingSummaryDto(
                ratings.Count,
                Math.Round(Avg(r => r.TitleScore, r => r.HookScore, r => r.HashtagScore, r => r.PracticalScore, r => r.OverallQualityScore), 2),
                Math.Round(Avg(r => r.PlatformFitScore, r => r.PlatformInfluenceScore, r => r.HashtagPlatformScore, r => r.PlatformOptimizationScore), 2),
                Math.Round(Avg(r => r.UsabilityScore, r => r.LayoutScore, r => r.PresentationScore), 2),
                Math.Round(Avg(r => r.TimeSavingScore, r => r.GenerationSpeedScore, r => r.SupportScore, r => r.EfficiencyComparisonScore), 2),
                Math.Round(Avg(r => r.OverallSatisfactionScore, r => r.ReusabilityScore, r => r.RecommendationScore), 2)
            );

            return Ok(summary);
        }

        // POST /api/ratings — Save rating
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRatingRequest req)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            if (await _repo.HasUserRatedAsync(userId))
                return Conflict("A rating already exists for this user.");

            var evaluation = new Evaluation(
                userId,
                req.VideosPerMonth, req.MainPlatform, req.Experience,
                req.TitleScore, req.HookScore, req.HashtagScore, req.PracticalScore, req.OverallQualityScore,
                req.PlatformFitScore, req.PlatformInfluenceScore, req.HashtagPlatformScore, req.PlatformOptimizationScore,
                req.UsabilityScore, req.LayoutScore, req.PresentationScore,
                req.TimeSavingScore, req.GenerationSpeedScore, req.SupportScore, req.EfficiencyComparisonScore,
                req.OverallSatisfactionScore, req.ReusabilityScore, req.RecommendationScore,
                req.Comment);

            await _repo.AddAsync(evaluation);
            return StatusCode(201, new { evaluation.Id, evaluation.CreatedAt });
        }
    }
}