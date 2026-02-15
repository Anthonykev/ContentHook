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
    public class TranscriptRepository : ITranscriptRepository
    {
        private readonly AppDbContext _db;

        
        public TranscriptRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Transcript> AddAsync(Transcript transcript)
        {
            _db.Transcripts.Add(transcript);
            await _db.SaveChangesAsync();
            return transcript;
        }

        public async Task<Transcript?> GetByIdAsync(Guid id)
        {
            return await _db.Transcripts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Transcript>> GetAllAsync()
        {
            return await _db.Transcripts
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
