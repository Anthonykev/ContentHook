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
    public class RatingRepository : IRatingRepository
    {
        private readonly AppDbContext _db;

        public RatingRepository(AppDbContext db) => _db = db;

        public async Task<Evaluation> AddAsync(Evaluation evaluation)
        {
            _db.Evaluations.Add(evaluation);
            await _db.SaveChangesAsync();
            return evaluation;
        }

        public async Task<List<Evaluation>> GetAllAsync()
            => await _db.Evaluations
                .AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

        public async Task<bool> HasUserRatedAsync(string userId)
            => await _db.Evaluations.AnyAsync(e => e.UserId == userId);

        public async Task DeleteByUserIdAsync(string userId)
        {
            var existing = await _db.Evaluations.FirstOrDefaultAsync(e => e.UserId == userId);
            if (existing is not null)
            {
                _db.Evaluations.Remove(existing);
                await _db.SaveChangesAsync();
            }
        }
    }
}
