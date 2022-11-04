﻿using Functionland.FxFiles.Client.Shared.Services;

namespace Functionland.FxFiles.Client.Shared.TestInfra.Implementations
{
    public partial class FakeFileServicePlatformTest_CreateTypical : FileServicePlatformTest
    {
        [AutoInject] public FakeFileServiceFactory FakeFileServiceFactory { get; set; } = default!;
        public override string Title => "Typical FakeFileService Test";

        public override string Description => "Tests the typical features of this FakeFileService";

        protected override IFileService OnGetFileService()
        {
            return FakeFileServiceFactory.CreateTypical(TimeSpan.Zero, TimeSpan.Zero);
        }

        protected override string OnGetTestsRootPath() => "fakeroot";
    }
}
