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

        public async Task<Transcript> CreateAsync(string userId, string text, string? language)
        {
         
            var transcript = new Transcript(userId, text, language);

            
            return await _repo.AddAsync(transcript);
        }

        public async Task<Transcript?> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<List<Transcript>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }
    }
}
