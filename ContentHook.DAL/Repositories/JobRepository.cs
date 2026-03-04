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
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _db;

        public JobRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Job> AddAsync(Job job)
        {
            _db.Jobs.Add(job);
            await _db.SaveChangesAsync();
            return job;
        }

        public async Task<Job?> GetByIdAsync(Guid id)
        {
            return await _db.Jobs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Job> UpdateAsync(Job job)
        {
            _db.Jobs.Update(job);
            await _db.SaveChangesAsync();
            return job;
        }
    }
}
