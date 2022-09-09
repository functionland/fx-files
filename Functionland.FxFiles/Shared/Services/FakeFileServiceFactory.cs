using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Services
{
    public static class FakeFileServiceFactory
    {

        public static FakeFileService CreateSimpleFileListOnRoot()
        {
            return new FakeFileService(new List<FsArtifact>
            {
                CreateFile("/image summer.jpg"),
                CreateFile("/image germany.jpg"),
                CreateFile("/proposal v1-2.pdf"),
            }
            );
        }

        public static FakeFileService CreateFolders()
        {
            return new FakeFileService(new List<FsArtifact>
            {
                CreateFolder("/images"),
                CreateFolder("/docs"),
                CreateFile("/images/image summer.jpg"),
                CreateFile("/images/image germany.jpg"),
                CreateFile("/docs/proposal v1-2.pdf"),
            }
            );
        }

        public static FakeFileService CreateNeste4dFolders()
        {
            return new FakeFileService(new List<FsArtifact>
            {
                CreateFolder("/images"),
                CreateFolder("/images/summer"),
                CreateFolder("/images/winter"),
                CreateFolder("/docs"),
                CreateFile("/images/summer/image summer.jpg"),
                CreateFile("/images/summer/image germany.jpg"),
                CreateFile("/images/winter/image summer.jpg"),
                CreateFile("/images/winter/image germany.jpg"),
                CreateFile("/docs/proposal v1-2.pdf"),
            }
            );
        }

        public static FakeFileService CreateTypical()
        {
            return new FakeFileService(new List<FsArtifact>
            {
                CreateFolder("/images"),
                CreateFolder("/images/summer"),
                CreateFolder("/images/winter"),
                CreateFolder("/docs"),
                CreateFile("/images/summer/image summer.jpg"),
                CreateFile("/images/summer/image germany.jpg"),
                CreateFile("/images/winter/image summer.jpg"),
                CreateFile("/images/winter/image germany.jpg"),
                CreateFile("/docs/proposal v1-2.pdf"),
            }
            );
        }

        public static FsArtifact CreateFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var extension = Path.GetExtension(filePath);

            return new FsArtifact()
            {
                FullPath = filePath,
                Name = fileName,
                FileExtension = extension,
                ArtifactType = FsArtifactType.File,
            };
        }

        public static FsArtifact CreateFolder(string folderPath)
        {
            var folderName = Path.GetDirectoryName(folderPath);

            return new FsArtifact()
            {
                FullPath = folderPath,
                Name = folderName,
                ArtifactType = FsArtifactType.Folder,
            };
        }

        public static FsArtifact CreateDrive(string drivePath)
        {
            var driveName = drivePath;

            return new FsArtifact()
            {
                FullPath = drivePath,
                Name = driveName,
                ArtifactType = FsArtifactType.Drive,
            };
        }
    }
}
