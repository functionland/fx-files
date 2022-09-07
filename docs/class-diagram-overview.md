# Basic Models
```mermaid
classDiagram
class FxFsArtifact{
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
FileService <|-- LocalDeviceFileService
LocalDeviceFileService <|-- AndroidInternalMemoryFileService
LocalDeviceFileService <|-- AndroidExternalMemoryFileService
LocalDeviceFileService <|-- WindowsFileService
FileService <|-- FulaFileService
FulaFileService <|-- AndroidFulaFileService
FulaFileService <|-- IosFulaFileService
FulaFileService <|-- WindowsFulaFileService

class FileService {
  <<abstract>>
  ctor(string dId)
  string Title
  FileProviderType ProviderType
  GetFiles(string path, string search = null, bool includeSubfolders = false) FxFsArtifact[]
  CreateFolder(string path, string folder) FxFsArtifact
  <<event>>
  event ArtifactsCreated()
  event ArtifactsDeleted()
  event ArtifactsModified()
}

class LocalDeviceFileService {
  <<abstract>>
  
}
```

# UI Components


```mermaid
classDiagram
class FileExplorerComponent {
  +IFileService[] FileService
}
```
