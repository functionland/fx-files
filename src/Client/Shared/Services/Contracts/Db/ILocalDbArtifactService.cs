using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Contracts
{
    public interface ILocalDbArtifactService
    {
        Task<List<FsArtifact>> GetChildrenArtifactsAsync(string localPath);

        Task<FsArtifact> GetArtifactAsync(string localPath);
    }
}
