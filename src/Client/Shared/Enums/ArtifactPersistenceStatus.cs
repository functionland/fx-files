using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Enums
{
    public enum ArtifactPersistenceStatus
    {
        PendingToUpload = 1,
        Uploading = 2,
        Done = 3,
        PendingToDownload = 4,
        Downloading = 5
    }
}
