# FulaClient
## Enums
Source codes:
- [FsArtifactType](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Enums/FsArtifactType.cs)
- [ArtifactPermissionLevel](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Enums/ArtifactPermissionLevel.cs)
- [ActionType](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Enums/ActionType.cs)
```mermaid
classDiagram

class FsArtifactType{
    <<Enum>>
    File
    Folder
    Drive
}

class ArtifactPermissionLevel{
    <<Flags>>
    None = 0
    Read = 1
    Write = 2
    Delegate = 4
}

class ActionType{
    <<Enum>>
    Created
    Modified
    Shared
    UnShared
    Commented
}
```

## Models
There are different models declared to use while working with FulaClient libraries. Here is a brief description about these models.
### Blox and Pool
Source codes:
- [Blox](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Models/Blox.cs)
- [BloxPool](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Models/BloxPool.cs)
- [BloxPoolPurchaseInfo](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Models/BloxPoolPurchaseInfo.cs)
```mermaid
classDiagram

class Blox{
    + string Id
    + string Name
    + string OwnerId
}

class BloxPool{
    + string Id
    + int? PingTime
    + KeyValuePair~string, string~[]? PrimaryInfos
    + KeyValuePair~string,KeyValuePair[]~[]? KeyValueGroups
}

class BloxPoolPurchaseInfo{
    + double? DueNowPaymentRequired
    + double? PerMounthPaymentRequired
}

```
### User
Source codes:
- [FulaUser](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Models/FulaUser.cs)
- [UserToken](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Models/UserToken.cs)
```mermaid
classDiagram

class FulaUser{
    + string DId
    + string? Username
    + bool IsParent
    + KeyValuePair~string, string~[] Claims
}

class UserToken{
    string Token,
    KeyValuePair~string, string~[] Claims
}
```
### File and Folder
Source codes:
- [FsArtifact](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Models/FsArtifact.cs)
- [FsArtifactActivity](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Models/FsArtifactActivity.cs)
- [ArtifactPermissionInfo](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Models/ArtifactPermissionInfo.cs)
```mermaid
classDiagram

class FsArtifact{
    + long?  Id
    + string Name
    + string FullPath,
    + string? ParentFullPath
    + FsArtifactType ArtifactType
    + long? Size
    + string? OriginDevice
    + DateTimeOffset CreateDateTime
    + DateTimeOffset LastModifiedDateTime
    + string? WhoMadeLastEdit
    + ArtifactPermissionInfo[]? PermissionedUsers 
    + string OwerDid
    + ArtifactPermissionLevel ArtifactPermissionLevel
    + bool? IsSharedWithMe
    + bool? IsSharedByMe
    + string ContentHas
}

class FsArtifactActivity{
    + FulaUser? Performer
    + DateTimeOffset? ActionDateTime
    + ActionType? ActionType
    + KeyValuePair~string,string~[]? Properties
}

class ArtifactPermissionInfo{
    + string FullPath
    + string DId
    + ArtifactPermissionLevel PermissionLevel
}
```
### Actions
Source codes:
- [ProgressInfo](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Components/Modal/ProgressModal/ProgressInfo.cs).
```mermaid
classDiagram

class ProgressInfo{
    + string? CurrentText
    + string? CurrentSubText
    + int? CurrentValue
    + int? MaxValue
}
```

## Interfaces
### Introduction
There are four interfaces for dealing with Fula.
 - FulaFileClient
 - FulaIdentityClient
 - FulaDatabaseClient
 - FulaBloxClient
 

```mermaid
classDiagram

class FulaFileClient {
    <<interface>>
}

class FulaIdentityClient {
    <<interface>>
}

class FulaDatabaseClient {
    <<interface>>
}

class FulaBloxClient {
    <<interface>>
}
```

### FulaBloxClient
To work with Bloxes in the Fula network there is a [FulaBloxClient](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Services/Contracts/FulaClient/IFulaBloxClient.cs).

### FulaFileClient
To work with files in the Fula network there is a [FulaFileClient](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Services/Contracts/FulaClient/IFulaFileClient.cs).

**Note:** These methods `GetChildrenArtifactsAsync`, `SearchArtifactsAsync`,`GetArtifactAsync` and `GetSharedByMeArtifacsAsync` which return `FsArtifact` should fill the following properies to keep the contract.
 - `Id `
 - `Name`
 - `FullPath `
 - `ParentFullPath `
 - `FileExtension`
 - `ArtifactType`
 - `Size`
 - `LastModifiedDateTime`
 - `IsSharedWithMe`
 - `IsSharedByMe`
 - `ContentHash`

All of the `FsArtifact` properties will be provided by `GetArtifactMetaAsync`.
- `Id`
- `Name`
- `FullPath`
- `ParentFullPath`
- `ArtifactType`
- `Size`
- `OriginDevice`
- `CreateDateTime`
- `LastModifiedDateTime`
- `WhoMadeLastEdit`
- `PermissionedUsers`
- `OwerDId`
- `ArtifactPermissionLevel`
- `IsSharedWithMe`
- `IsSharedByMe`
- `ContentHas`


### FulaIdentityClient
To work with users and everything related to identity of the suers in the Fula network there is a [FulaIdentityClient](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Services/Contracts/FulaClient/IFulaIdentityClient.cs).

### FulaDatabaseClient
To work with the GraphQL database provided by Fula network there is a [FulaDatabaseClient](https://github.com/functionland/fx-files/blob/main/src/Client/Shared/Services/Contracts/FulaClient/IFulaDatabaseClient.cs).

