using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentHook.DAL.Entities;

namespace ContentHook.BL.Interfaces
{
    public interface ITranscriptService
    {
        Task<Transcript> CreateAsync(string userId, string text, string? language, string? originalFileName);
        Task<Transcript?> GetByIdAsync(Guid id);
        Task<List<Transcript>> GetAllAsync();
    }
}
