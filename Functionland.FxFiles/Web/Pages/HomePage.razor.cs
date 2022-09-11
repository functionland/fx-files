using System.Runtime.CompilerServices;

namespace Functionland.FxFiles.App.Pages;

public partial class HomePage
{
    public IFileService FileService { get; set; }

    public async Task Test()
    {
        var list =  FileService.GetArtifactsAsync();
        await foreach(var item in list)
        {
            Console.WriteLine("Test");
            
        }
    }
}

