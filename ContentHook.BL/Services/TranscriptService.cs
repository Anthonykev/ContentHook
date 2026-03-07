using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentHook.BL.Interfaces;
using ContentHook.DAL.Entities;
using ContentHook.DAL.Interfaces;

namespace ContentHook.BL.Services
{
    public class TranscriptService : ITranscriptService
    {
        private readonly ITranscriptRepository _repo;

        public TranscriptService(ITranscriptRepository repo)
        {
            _repo = repo;
        }

        public async Task<Transcript> CreateAsync(
            string userId,
            string text,
            string? language,
            string? originalFileName)
        {
            var transcript = new Transcript(userId, text, language, originalFileName);
            return await _repo.AddAsync(transcript);
        }

        public async Task<Transcript?> GetByIdAsync(Guid id)
            => await _repo.GetByIdAsync(id);

        public async Task<List<Transcript>> GetAllAsync()
            => await _repo.GetAllAsync();

        public async Task<Transcript> UpdateTextAsync(Guid id, string userId, string newText)
        {
            var transcript = await _repo.GetByIdAsync(id);
            if (transcript is null)
                throw new KeyNotFoundException($"Transcript {id} not found.");
            if (transcript.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            transcript.UpdateText(newText);
            return await _repo.UpdateAsync(transcript);
        }
    }
}
