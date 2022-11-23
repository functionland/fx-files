using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface ILocalDbPinService
{
    Task AddPinAsync(FsArtifact artifact);
    Task<List<PinnedArtifact>> GetPinnedArticatInfos();
    Task RemovePinAsync(string FullPath);
    Task UpdatePinAsync(PinnedArtifact pinnedArtifact, string? oldPath = null);
}
