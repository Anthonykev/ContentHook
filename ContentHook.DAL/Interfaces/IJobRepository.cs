using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentHook.DAL.Entities;

namespace ContentHook.DAL.Interfaces
{
    public interface IJobRepository
    {
        Task<Job> AddAsync(Job job);
        Task<Job?> GetByIdAsync(Guid id);
        Task<Job> UpdateAsync(Job job);
    }
}
