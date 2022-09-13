# Basic Models
The `FsArtifact` is an entity to describe a *File*, *Folder* or a *Drive* in the file system in any platform. The file system platform could be an android's internal memory, a windows drive, or a Blox Storage. All these file systems are stroing `FsArtifact`s which we may call them as **artifact**(s) in this document.
```mermaid
classDiagram
class FsArtifact{
  int Id
  string FullPath
  string Name
  string FileExtension
  FsArtifactType
  FileProviderType ProviderType
  int Size
  string ContentHash
  int ParentId
  string ZoneName
  bool IsSharedByMe
  bool IsSharedWithMe
  string OwnerId
  string Thumbnail
  string OriginDevice
  bool IsAvailableOffline
  bool IsPinned
}

class FileProviderType{
  <<enumeration>>
  InternalMemory
  ExternalMemory
  Blox
}

class FsArtifactType{
  <<enumeration>>
  Folder
  File
  Drive
}
```

# FileService Architecture
```mermaid
classDiagram
IFileService <|-- LocalDeviceFileService
LocalDeviceFileService <|-- AndroidFileService
LocalDeviceFileService <|-- IosFileService
LocalDeviceFileService <|-- WindowsFileService
IFileService <|-- FakeFileService
IFileService <|-- FulaFileService
FulaFileService <|-- AndroidFulaFileService
FulaFileService <|-- IosFulaFileService
FulaFileService <|-- WindowsFulaFileService

class IFileService {
  <<interface>>
  GetFiles(string path, string search = null, bool includeSubfolders = false) FxFsArtifact[]
  CreateFile(string path, Stream fileStream)
  GetFile(string path) Stream
  CreateFolder(string path, string folder) FxFsArtifact
}

class LocalDeviceFileService {
  <<abstract>>
}
```

# UI Components


```mermaid
classDiagram
class FileExplorerComponent {
  +IFileService FileService
}
```
