﻿using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class ImageThumbnailPlugin : IThumbnailPlugin
{
    public bool IsJustFilePathSupported => false;

    public Task<Stream> CreateThumbnailAsync(Stream? stream, string? filePath, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null)
    {
        return OnCreateThumbnailAsync(stream, filePath, thumbnailScale, cancellationToken);
    }

    protected abstract Task<Stream> OnCreateThumbnailAsync(Stream? stream, string? filePath, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null);

    public bool IsSupported(string extension)
    {
        return FsArtifactUtils.FileExtentionsType
                        .Where(e => e.Value == FileCategoryType.Image)
                        .Select(f => f.Key)
                        .Any(c => c.Equals(extension.ToLower()));
    }
}
