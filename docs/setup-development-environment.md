# Setup the Development Environment

## Windows requirements
- **Windows Version:** You need to have Windows 11 or Windows 10 (19043 or higher)
- **Hyper-V:** You to have Hyper-V enabled. 

![Install Hyper-V](https://github.com/functionland/fx-files/blob/main/docs/images/hyperv.png)

Follow [this documentation](https://learn.microsoft.com/en-us/xamarin/android/get-started/installation/android-emulator/hardware-acceleration?tabs=vswin&pivots=windows#hyper-v) to enable it on your system. 
You can enable it by this command too (after you've enabled the hardware in BIOS seetings):
```
		DISM /Online /Enable-Feature /All /FeatureName:Microsoft-Hyper-V
```

## Visual Studio
- Visual Studio 17.4.0 or higher
- Workloads:
    - ASP.NET and web development
	- .NET Multi-platform App UI development
    - .NET desktop development
- .NET SDK 6.0.400: You can install it by 
```cmd
winget install Microsoft.DotNet.SDK.6 --version 6.0.400
```

- Make sure Nuget is configured.

![Configure Nuget](https://github.com/functionland/fx-files/blob/main/docs/images/nuget.png)

- Restore all nuget packages

- Run this command:
```cmd
dotnet workload restore
```

- Install this extensions:
  - Web Compiler 2022+

![Web Compiler 2022+](https://github.com/functionland/fx-files/blob/main/docs/images/webcompiler.png)

- Run these batch files in the project:
  - Clean.bat
  - CleanCSS.bat

- Right click on the solution and click on **Re-compile all files in solution**

![Re-compile all files in solution](https://github.com/functionland/fx-files/blob/main/docs/images/recompile.png)
- Rebuild the project

## You are ready!

