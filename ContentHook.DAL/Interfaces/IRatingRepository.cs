using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentHook.DAL.Entities;

namespace ContentHook.DAL.Interfaces
{
    public interface IRatingRepository
    {
        Task<Evaluation> AddAsync(Evaluation evaluation);
        Task<List<Evaluation>> GetAllAsync();
        Task<bool> HasUserRatedAsync(string userId);
        Task DeleteByUserIdAsync(string userId);
    }
}