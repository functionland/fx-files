using Functionland.FxFiles.Client.Shared.Utils;
using NAudio.Wave;
using NAudio.WaveFormRenderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using Color = System.Drawing.Color;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.Thumbnail.Plugins
{
    public abstract class AudioThumbnailPlugin : IThumbnailPlugin
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
                            .Where(e => e.Value == FileCategoryType.Audio)
                            .Select(f => f.Key)
                            .Any(c => c.Equals(extension.ToLower()));
        }

        
    }
}
