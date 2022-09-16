using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Functionland.FxFiles.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Shared.Services.Implementations
{
    public abstract partial class PlatformTestService : IPlatformTestService
    {

        protected virtual List<IPlatformTest> OnGetTests()
        {
            return new List<IPlatformTest>()
            {
            };
        }

        public IEnumerable<IPlatformTest> GetTests()
        {
            return OnGetTests();
        }

        public async Task RunTestAsync(IPlatformTest platformTest)
        {
            try
            {
                platformTest.ProgressChanged += OnTestProgressChanged;
                await platformTest.RunAsync();
            }
            finally
            {
                platformTest.ProgressChanged -= OnTestProgressChanged;
            }
        }

        public void OnTestProgressChanged(object? sender, TestProgressChangedEventArgs eventArgs)
        {
            TestProgressChanged?.Invoke(sender, eventArgs);
        }

        public event EventHandler<TestProgressChangedEventArgs>? TestProgressChanged;
    }


}
