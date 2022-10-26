﻿namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FileDetailView
    {
        [Parameter]
        public bool IsImageFile { get; set; } = false;

        [Parameter]
        public string? FileName { get; set; }

        [Parameter]
        public string? FilePath { get; set; }

        [Parameter]
        public string? FileDetailAreaClass { get; set; }

        [Parameter]
        public string? ContentBodyClass { get; set; }

        [Parameter]
        public FileCategoryType FileCategoryType { get; set; }

        [Parameter]
        public FsArtifactType FsArtifactType { get; set; }

        [Parameter]
        public RenderFragment? FileDetailItemFragment { get; set; } = default!;

        [Parameter]
        public RenderFragment? FileDetailItemActionsFragment { get; set; } = default!;

        [Parameter]
        public RenderFragment? FileDetailBottomActionFragment { get; set; } = default!;

        [Parameter]
        public IFileService? FileService { get; set; }

        public PathProtocol Protocol =>
            FileService switch
            {
                ILocalDeviceFileService => PathProtocol.ThumbnailStorageMedium,
                IFulaFileService => PathProtocol.ThumbnailFulaMedium,
                _ => throw new InvalidOperationException($"Unsupported file service: {FileService}")
            };
    }
}
