using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Functionland.FxFiles.App.Pages;

public partial class HomePage
{
    [AutoInject] public IFileService FileService { get; set; }
    private void NavigateToTestExplorerComponent()
    {
        NavigationManager.NavigateTo("/TestExplorer");
    }
    public async Task Test()
    {
        var list = FileService.GetArtifactsAsync();
        await foreach (var item in list)
        {
            Console.WriteLine("Test");

        }
    }
}

