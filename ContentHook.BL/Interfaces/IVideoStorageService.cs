using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentHook.BL.Interfaces
{
    public interface IVideoStorageService
    {
        /// Speichert Video und gibt Storage-Key zurück
        Task<string> SaveAsync(
            Stream videoStream,
            string fileName,
            CancellationToken cancellationToken = default);

        /// Gibt Video als Stream zurück
        Task<Stream> GetAsync(
            string storageKey,
            CancellationToken cancellationToken = default);

        /// Löscht Video nach Verarbeitung
        Task DeleteAsync(string storageKey);
    }
}