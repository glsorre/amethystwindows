C:/"Program Files (x86)"/"Microsoft Visual Studio"/2019/Enterprise/MSBuild/Current/Bin/msbuild.exe "$($env:APPCENTER_SOURCE_DIRECTORY)\AmethystWindows\AmethystWindows.csproj" /p:Configuration="$($env:APPCENTER_UWP_CONFIGURATION)" /p:Platform=ARM

C:/"Program Files (x86)"/"Microsoft Visual Studio"/2019/Enterprise/MSBuild/Current/Bin/msbuild.exe "$($env:APPCENTER_SOURCE_DIRECTORY)\AmethystWindows\AmethystWindows.csproj" /p:Configuration="$($env:APPCENTER_UWP_CONFIGURATION)" /p:Platform=x86

C:/"Program Files (x86)"/"Microsoft Visual Studio"/2019/Enterprise/MSBuild/Current/Bin/msbuild.exe "$($env:APPCENTER_SOURCE_DIRECTORY)\AmethystWindows\AmethystWindows.csproj" /p:Configuration="$($env:APPCENTER_UWP_CONFIGURATION)" /p:Platform=x64
