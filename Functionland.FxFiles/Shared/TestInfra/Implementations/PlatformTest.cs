namespace Functionland.FxFiles.Shared.TestInfra.Implementations
{
    public abstract class PlatformTest : IPlatformTest
    {
        public PlatformTest()
        {
            Assert = new PlatformTestAssertion(Progress);
        }

        public abstract string Title { get; }

        public abstract string Description { get; }

        public event EventHandler<TestProgressChangedEventArgs>? ProgressChanged;
        protected PlatformTestAssertion Assert { get; }
        protected void OnTestProgressChanged(object? sender, TestProgressChangedEventArgs eventArgs)
        {
            ProgressChanged?.Invoke(this, eventArgs);
        }

        protected void Progress(string title, string? description, TestProgressType progressType)
        {
            OnTestProgressChanged(this, new TestProgressChangedEventArgs(title, description, progressType));
        }

        protected abstract Task OnRunAsync();

        public async Task RunAsync()
        {
            await OnRunAsync();
        }

        public class PlatformTestAssertion
        {
            public PlatformTestAssertion(Action<string, string?, TestProgressType> onAssert)
            {
                this.onAssert = onAssert;
            }

            Action<string, string?, TestProgressType> onAssert { get; set; }
            public void IsTrue(bool? actual, string title, string? description = null)
            {
                if (actual == true)
                {
                    onAssert(title, description, TestProgressType.Success);
                }
                else
                {
                    onAssert(title, description, TestProgressType.Fail);
                }
            }

            public void IsFalse(bool? actual, string title, string? description = null)
            {
                if (actual == false)
                {
                    onAssert(title, description, TestProgressType.Success);
                }
                else
                {
                    onAssert(title, description, TestProgressType.Fail);
                }
            }

            public void IsNull<T>(T actual, string title, string? description = null)
            {
                if (actual is null)
                {
                    onAssert(title, description, TestProgressType.Success);
                }
                else
                {
                    onAssert(title, description, TestProgressType.Fail);
                }
            }

            public void IsNotNull<T>(T actual, string title, string? description = null)
            {
                if (actual is not null)
                {
                    onAssert(title, description, TestProgressType.Success);
                }
                else
                {
                    onAssert(title, description, TestProgressType.Fail);
                }
            }


            public void AreEqual<T>(T expected, T actual, string title, string? description = null)
                where T : notnull
            {
                if (actual.Equals(expected))
                {
                    onAssert(title, description, TestProgressType.Success);
                }
                else
                {
                    onAssert(title, $"Expected: {expected}, Actual: {actual}", TestProgressType.Fail);
                }
            }

            public void Success(string title, string? description = null)
            {
                onAssert(title, description, TestProgressType.Success);
            }

            public void Fail(string title, string? description = null)
            {
                onAssert(title, description, TestProgressType.Fail);
            }

            public async Task ShouldThrowAsync<TException>(Func<Task> action, string title, string? description = null)
                where TException : Exception
            {
                try
                {
                    await action();
                    onAssert(title, "Unexpectedly no exception occured.", TestProgressType.Fail);
                }
                catch (TException exception)
                {
                    onAssert(title, exception.ToString(), TestProgressType.Success);
                }
                catch (Exception exception)
                {
                    onAssert(title,
                        $"Wrong ExceptionType. Exptected: '{typeof(TException).Name}', Actual: {exception.GetType().Name}",
                        TestProgressType.Fail);
                }
            }
        }
    }
}
