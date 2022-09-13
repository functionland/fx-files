using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Functionland.FxFiles.App.Pages;

public partial class HomePage
{
    private string internalfilePath2;

    [AutoInject] public IFileService FileService { get; set; }
    private void NavigateToTestExplorerComponent()
    {
        NavigationManager.NavigateTo("/TestExplorer");
    }
    public async Task AndroidFileServiceTest()
    {
        //  sample paths:
        var sDPath = "/storage/1EED-3A0B/Alarms";
        var sDfilePath = "/storage/1EED-3A0B/Alarms/IMG_20220912_050004-Copy01.jpg";
        var internalPath = "/storage/emulated/0/Pictures/TestCreateFolder";
        var internalfilePath = "/storage/emulated/0/Pictures/IMG_20220912_050004.jpg";
        var internalfilePath2 = "/storage/emulated/0/Audiobooks/IMG_20220912_050004.jpg";
        
        //############################################
        //GetArtifactsAsync
        //var list = FileService.GetArtifactsAsync();
        var list = FileService.GetArtifactsAsync(internalPath);
        //var list = FileService.GetArtifactsAsync(sDPath); 
        var results = new List<FsArtifact>();

        await foreach (var element in list)
        {
            Console.WriteLine("Test: " + element);
            results.Add(element);
        }
        _ = results;

        //############################################
        //GetFileContentAsync
        //var InternalFileContentStream = await FileService.GetFileContentAsync(internalfilePath);
        //var sDFileContentStream = await FileService.GetFileContentAsync(sDfilePath);

        //############################################ ((((test failed))))
        //CreateFileAsync 
        //var fileArtifact = await FileService.CreateFileAsync(internalfilePath2, InternalFileContentStream);

        //############################################
        //CreateFolderAsync
        //var folderArtifact = await FileService.CreateFolderAsync(internalPath, "TestCreateFolder");
        //var folderArtifact = await FileService.CreateFolderAsync(sDPath, "TestCreateFolder");

        //###########################################((((test failed))))
        //DeleteArtifactsAsync
        //await FileService.DeleteArtifactsAsync(results.ToArray());

        //###########################################((((test failed))))
        //
        var des = "/storage/emulated/0/Pictures/";
        await FileService.CopyArtifactsAsync(results.ToArray(), des);
    }
}

