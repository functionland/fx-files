using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Services.Contracts
{
    public interface IThumbnailService
    {
        Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null);
    }
}
