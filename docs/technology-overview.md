# Technology Architecture

This document describes the technologies used in FxFiles App.

## Technology Requirements
FxFiles is going to be published for all prominent platforms: *Windows*, *Android*, *iOS* and *Mac*.
To support all these platforms we have decided to use a cross-platform development options.

Another fact to mention is the UX designs for all platforms are the same. So there are not different designs for Android, iOS or Windows.
So we need to implement exact same UIs for each platform. This fact makes using non-native controls a better choice.

## Technolgoy Selection
Based on these facts came in "Technology Requirements", we have picked .NET MAUI Blazor so we can use HTML/CSS to implements UIs consistent UIs amongst different platforms.
Also we can use C# to implement all the logic. Using C# we could write both *shared logic* and *platform-specific logic* in the same language.
