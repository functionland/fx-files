CREATE TABLE PinnedArtifact (
    Id INTEGER PRIMARY KEY NOT NULL,
    FullPath TEXT NULL,
    ThumbnailPath TEXT NULL,
    ContentHash TEXT NULL,
    ProviderType INTEGER NULL,
    PinEpochTime INTEGER NULL,
    FsArtifactType INTEGER NULL,
    ArtifactName TEXT NULL
)