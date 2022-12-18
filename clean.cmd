@echo off

if not defined VSCMD_VER (
    echo %COLOR_WARN%This script must be run inside Developer Command Prompt%COLOR_NORM%
    goto :end
)

if /i "%1" == "debug" (
    set BUILD_TYPE=Debug
)

if /i "%1" == "release" (
    set BUILD_TYPE=Release
)

if not defined BUILD_TYPE (
    echo Usage: %0 Debug^|Release
    goto end
)

MSBuild /t:Clean /p:Configuration=%BUILD_TYPE% /p:Platform="Any CPU" || goto err
MSBuild /t:Clean /p:Configuration=%BUILD_TYPE% /p:Platform=x86 || goto err
MSBuild /t:Clean /p:Configuration=%BUILD_TYPE% /p:Platform=x64 || goto err

goto end

:err
echo %COLOR_WARN%Aborting, last command failed with error code %ERRORLEVEL%%COLOR_NORM%

:end
set BUILD_TYPE=
set COLOR_NORM=
set COLOR_WARN=