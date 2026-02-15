using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentHook.DAL.Entities;

namespace ContentHook.DAL.Interfaces
{
    public interface ITranscriptRepository
    {
        Task<Transcript> AddAsync(Transcript transcript);

        Task<Transcript?> GetByIdAsync(Guid id);

        Task<List<Transcript>> GetAllAsync();
    }
}
