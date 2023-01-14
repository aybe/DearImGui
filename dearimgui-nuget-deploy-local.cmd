@echo off

set DEARIMGUI_NUGET_BUILD=build\packages\AnyCPU\Release
if not exist %DEARIMGUI_NUGET_BUILD% (
    echo The directory "%DEARIMGUI_NUGET_BUILD%" could not be found, aborting...
    echo Have you built the project for Release configuration?
    goto :end
)

set DEARIMGUI_NUGET_CACHE=%USERPROFILE%\.nuget\packages
echo Pruning packages from local package cache "%DEARIMGUI_NUGET_CACHE%"...
call :delete_directory "%DEARIMGUI_NUGET_CACHE%\dearimgui"
call :delete_directory "%DEARIMGUI_NUGET_CACHE%\dearimgui.opentk"
call :delete_directory "%DEARIMGUI_NUGET_CACHE%\dearimplot"

set DEARIMGUI_NUGET_LOCAL=C:\NuGet
echo Copying packages to local nuget directory "%DEARIMGUI_NUGET_LOCAL%"...
if not exist %DEARIMGUI_NUGET_LOCAL% md %DEARIMGUI_NUGET_LOCAL%
pushd %DEARIMGUI_NUGET_BUILD%
copy /y *.nupkg %DEARIMGUI_NUGET_LOCAL%
popd

goto :end

:delete_directory
if exist %1 rd /s /q %1 & echo Removed %1
goto :eof

:end
set DEARIMGUI_NUGET_BUILD=
set DEARIMGUI_NUGET_CACHE=
set DEARIMGUI_NUGET_LOCAL=