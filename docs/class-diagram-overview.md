# Basic Models
The `FsArtifact` is an entity to describe a *File*, *Folder* or a *Drive* in the file system in any platform. The file system platform could be an android's internal memory, a windows drive, or a Blox Storage. All these file systems are strong `FsArtifact`s which we may call as **artifact**(s) in this document.
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
To unify the development experience of facing with different file systems (Android, iOS, Windows, Blox and ...) we use an abstraction called `IFileService`. This abstraction represents all the requirements that a typical file system should expose.

As you see, there are different implementations of `IFileService` for different platforms leveraging specialized API(s) of each specific platform.
Amongst these implementations `FakeFileService` is the interesting one for developers, as they can use it to easily test their application, removing all the barriers to setup a proper file system for testing purposes.
```mermaid
classDiagram
IFileService <|-- ILocalDeviceFileService
ILocalDeviceFileService <|-- LocalDeviceFileService
LocalDeviceFileService <|-- AndroidFileService
LocalDeviceFileService <|-- IosFileService
LocalDeviceFileService <|-- WindowsFileService
ILocalDeviceFileService <|-- FakeFileService
IFulaFileService <|-- FakeFileService
IFileService <|-- IFulaFileService
IFulaFileService <|-- FulaFileService

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
# PinService Architecture
```mermaid
classDiagram
IPinService <|-- ILocalDevicePinService
ILocalDevicePinService <|-- LocalDevicePinService
IPinService <|-- IFulaPinService
IFulaPinService <|-- FulaPinService
```
# OfflineAvailablityService Architecture
```mermaid
classDiagram
IOfflineAvailablityService <|-- WindowsOfflineAvailablityService
IOfflineAvailablityService <|-- AndroidOfflineAvailablityService
IOfflineAvailablityService <|-- IosOfflineAvailablityService
IOfflineAvailablityService <|-- FakeOfflineAvailablityService

class IOfflineAvailablityService{
<<interface>>
InitAsync(CancellationToken? cancellationToken = null)
EnsureInitializedAsync()
MakeAvailableOfflineAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
RemoveAvailableOfflineAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
IsAvailableOfflineAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
GetFulaLocalFolderAddress()
}
```

# ShareService Architecture
```mermaid
classDiagram
IShareService <|-- ILocalDeviceShareService
IShareService <|-- IFulaShareService
ILocalDeviceShareService <|-- LocalDeviceShareService
IFulaShareService <|-- FulaShareService

class IShareService{
<<interface>>
InitAsync(CancellationToken? cancellationToken = null)
EnsureInitializedAsync()
ShareFsArtifactAsync(IEnumerable<string> dids, FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
ShareFsArtifactsAsync(IEnumerable<string> dids, IEnumerable<FsArtifact> fsArtifact, CancellationToken? cancellationToken = null)
UnShareFsArtifactAsync(IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null)
UnShareFsArtifactsAsync(IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null)
GetSharedFsArtifactsAsync(CancellationToken? cancellationToken = null) FsArtifact
}
```
