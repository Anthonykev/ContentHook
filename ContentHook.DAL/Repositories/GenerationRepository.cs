using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContentHook.DAL.Entities;
using ContentHook.DAL.Interfaces;
using ContentHook.DAL.ORMapper;
using Microsoft.EntityFrameworkCore;

namespace ContentHook.DAL.Repositories
{
    public class GenerationRepository : IGenerationRepository
    {
        private readonly AppDbContext _context;

        public GenerationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Generation> AddAsync(Generation generation)
        {
            _context.Generations.Add(generation);
            await _context.SaveChangesAsync();
            return generation;
        }

        public async Task<Generation?> GetByIdAsync(Guid id)
            => await _context.Generations.FindAsync(id);

        public async Task<List<Generation>> GetByTranscriptIdAsync(Guid transcriptId)
            => await _context.Generations
                .Where(g => g.TranscriptId == transcriptId)
                .OrderBy(g => g.CreatedAt)
                .ToListAsync();

        public async Task<int> CountByTranscriptAndPlatformAsync(
            Guid transcriptId, string platform)
            => await _context.Generations
                .CountAsync(g => g.TranscriptId == transcriptId
                              && g.Platform == platform.ToLowerInvariant());
    }
}