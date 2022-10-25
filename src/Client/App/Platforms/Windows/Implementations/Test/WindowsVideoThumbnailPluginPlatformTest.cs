using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations.Test
{
    public class WindowsVideoThumbnailPluginPlatformTest<TFileService> : ArtifactThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
    {
        public WindowsVideoThumbnailPluginPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService, TFileService fileService) 
            : base(artifactThumbnailService, fileService)
        {
        }

        public override string Title => $"WindowsVideoThumbnailPluginPlatformTest {typeof(TFileService).Name}";

        public override string Description => "Test for create artifact thumbnail on windows";

        protected override string OnGetRootPath() => "c:\\";

        protected override Task OnRunThumbnailPluginAsync(string testRootPath)
        {
            throw new NotImplementedException();
        }
    }
}
