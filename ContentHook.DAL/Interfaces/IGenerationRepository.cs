using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentHook.DAL.Entities;

namespace ContentHook.DAL.Interfaces
{
    public interface IGenerationRepository
    {
        Task<Generation> AddAsync(Generation generation);
        Task<Generation?> GetByIdAsync(Guid id);
        Task<List<Generation>> GetByTranscriptIdAsync(Guid transcriptId);

        // Zählt wie oft generiert wurde (max 3)
        Task<int> CountByTranscriptAndPlatformAsync(
            Guid transcriptId, string platform);
    }
}