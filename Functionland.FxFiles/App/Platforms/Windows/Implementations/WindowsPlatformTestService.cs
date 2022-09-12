using Functionland.FxFiles.Shared.TestInfra.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.App.Platforms.Windows.Implementations
{
    public partial class WindowsPlatformTestService : PlatformTestService
    {
        [AutoInject] FakeFileServicePlatformTest FakeFileServicePlatformTest { get; set; }

        
        protected override List<IPlatformTest> OnGetTests()
        {
            return new List<IPlatformTest>()
            {
                FakeFileServicePlatformTest
            };
        }
      
    }
}
