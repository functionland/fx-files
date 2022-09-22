using Functionland.FxFiles.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Shared.TestInfra.Implementations;
using System.Text;

namespace Functionland.FxFiles.App.Pages;

public partial class TestExplorer
{
    [AutoInject] protected IPlatformTestService PlatformTestService { get; set; } = default!;

    private List<IPlatformTest> PlatformTests { get; set; } = new();
    public string TestName = "";
    private bool IsDescriptionOpen = false;
    private string Description = "";
    private List<TestProgressChangedEventArgs> testProgressChangedEventArgs = new();
    protected override Task OnInitAsync()
    {
        PlatformTests = PlatformTestService.GetTests().ToList();
        return base.OnInitAsync();
    }

    private void ShowDescription(TestProgressChangedEventArgs item)
    {
        IsDescriptionOpen = true;
        Description = item.Description ?? "";
    }

    private async Task HandleValidSubmit()
    {
        testProgressChangedEventArgs.Clear();
        var selectedTest = PlatformTests.Where(c => TestName.Equals(c.Title)).FirstOrDefault();
        if (selectedTest == null) return;
        try
        {
            selectedTest.ProgressChanged += OnTestProgressChanged;
            await PlatformTestService.RunTestAsync(selectedTest);
        }
        finally
        {
            selectedTest.ProgressChanged -= OnTestProgressChanged;
        }
    }

    private void OnTestProgressChanged(object? sender, TestProgressChangedEventArgs eventArgs)
    {
        InvokeAsync(() =>
        {
            testProgressChangedEventArgs.Add(eventArgs);
            StateHasChanged();
        });
    }
    private List<BitDropDownItem> GetDropdownItems()
    {
        var bitDropDownItems = new List<BitDropDownItem>();

        foreach (var platformTest in PlatformTests)
        {
            var bitDropDownItem = new BitDropDownItem
            {
                Text = platformTest.Title,
                Value = platformTest.Title,
            };
            bitDropDownItems.Add(bitDropDownItem);
        }

        return bitDropDownItems;
    }


}

