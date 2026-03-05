using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContentHook.DAL.Entities;

namespace ContentHook.BL.Interfaces
{
    public interface IGenerationService
    {
        Task<Generation> GenerateAsync(
            string userId,
            Guid transcriptId,
            string transcriptText,
            string platform,
            CancellationToken cancellationToken = default);
    }
}