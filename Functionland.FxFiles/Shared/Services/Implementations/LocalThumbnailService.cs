using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Services.Implementations
{
    public abstract class LocalThumbnailService : IThumbnailService
    {
        public abstract Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null);
    }
}
