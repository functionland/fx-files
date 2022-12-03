﻿using Microsoft.JSInterop;
using System.Reflection.Metadata;
using System.Text;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class TextViewer : IFileViewerComponent, IDisposable
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnPin { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnUnpin { get; set; }
    [Parameter] public EventCallback<FsArtifact> OnOptionClick { get; set; }

    [AutoInject] private ThemeInterop ThemeInterop = default!;

    public bool IsLoading { get; set; } = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("import", "./_content/Functionland.FxFiles.Client.Shared/lib/code-mirror/runmode-standalone.js");
            await JSRuntime.InvokeVoidAsync("import", "./_content/Functionland.FxFiles.Client.Shared/lib/code-mirror/addon/mode/loadmode.js");
            await JSRuntime.InvokeVoidAsync("import", "./_content/Functionland.FxFiles.Client.Shared/lib/code-mirror/addon/mode/multiplex.js");
            await JSRuntime.InvokeVoidAsync("import", "./_content/Functionland.FxFiles.Client.Shared/lib/code-mirror/addon/mode/overlay.js");
            await JSRuntime.InvokeVoidAsync("import", "./_content/Functionland.FxFiles.Client.Shared/lib/code-mirror/addon/mode/simple.js");
            await JSRuntime.InvokeVoidAsync("import", "./_content/Functionland.FxFiles.Client.Shared/lib/code-mirror/mode.js");

            var _isSystemThemeDark = await ThemeInterop.GetThemeAsync() is FxTheme.Dark;
            await JSRuntime.InvokeVoidAsync("setupCodeMirror", _isSystemThemeDark);
            _ = GetTextAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandlePinAsync()
    {
        if (CurrentArtifact is null) return;

        await OnPin.InvokeAsync(new List<FsArtifact>() { CurrentArtifact });
    }

    private async Task HandleUnpinAsync()
    {
        if (CurrentArtifact is null) return;

        await OnUnpin.InvokeAsync(new List<FsArtifact>() { CurrentArtifact });
    }

    private async Task HandleOptionClickAsync()
    {
        if (CurrentArtifact is null) return;

        await OnOptionClick.InvokeAsync(CurrentArtifact);
    }

    private async Task GetTextAsync()
    {
        if (CurrentArtifact?.FullPath == null) return;

        var stream = await FileService.GetFileContentAsync(CurrentArtifact.FullPath);
        if (stream is null)
            return;

        using (var sr = new StreamReader(stream))
        {
            var text = await sr.ReadToEndAsync();
            await JSRuntime.InvokeVoidAsync("setCodeMirrorText", text, CurrentArtifact.Name);
        }
       
        IsLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        JSRuntime.InvokeVoidAsync("unRegisterOnTouchEvent");
    }
}