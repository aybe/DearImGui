@echo off

set COLOR_WARN=[93m
set COLOR_NORM=[0m

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

echo %COLOR_WARN%Building DearImGui...%COLOR_NORM%

echo %COLOR_WARN%Restoring packages...%COLOR_NORM%
MSBuild /p:Configuration=%BUILD_TYPE% -t:restore || goto err

echo %COLOR_WARN%Building imgui %BUILD_TYPE%^|x86%COLOR_NORM%
MSBuild /p:Configuration=%BUILD_TYPE% /p:Platform=x86 /p:OutDir=..\build\x86\%BUILD_TYPE%\ imgui\imgui.vcxproj || goto err

echo %COLOR_WARN%Building imgui %BUILD_TYPE%^|x64%COLOR_NORM%
MSBuild /p:Configuration=%BUILD_TYPE% /p:Platform=x64 /p:OutDir=..\build\x64\%BUILD_TYPE%\ imgui\imgui.vcxproj || goto err

echo %COLOR_WARN%Building implot %BUILD_TYPE%^|x86%COLOR_NORM%
MSBuild /p:Configuration=%BUILD_TYPE% /p:Platform=x86 /p:OutDir=..\build\x86\%BUILD_TYPE%\ implot\implot.vcxproj || goto err

echo %COLOR_WARN%Building implot %BUILD_TYPE%^|x64%COLOR_NORM%
MSBuild /p:Configuration=%BUILD_TYPE% /p:Platform=x64 /p:OutDir=..\build\x64\%BUILD_TYPE%\ implot\implot.vcxproj || goto err

echo %COLOR_WARN%Building DearGenerator%COLOR_NORM%
MSBuild /p:Configuration=%BUILD_TYPE% DearGenerator\DearGenerator.csproj || goto err

echo %COLOR_WARN%Building DearImGui.Generator%COLOR_NORM%
MSBuild /p:Configuration=%BUILD_TYPE% DearImGui.Generator\DearImGui.Generator.csproj || goto err

echo %COLOR_WARN%Building DearImPlot.Generator%COLOR_NORM%
MSBuild /p:Configuration=%BUILD_TYPE% DearImPlot.Generator\DearImPlot.Generator.csproj || goto err

echo %COLOR_WARN%Running DearImGui.Generator%COLOR_NORM%
pushd DearImGui.Generator\bin\%BUILD_TYPE%\net6.0
DearImGui.Generator.exe || goto err
popd

echo %COLOR_WARN%Running DearImPlot.Generator%COLOR_NORM%
pushd DearImPlot.Generator\bin\%BUILD_TYPE%\net6.0
DearImPlot.Generator.exe || goto err
popd

echo %COLOR_WARN%Building DearImGui%COLOR_NORM%
MSBuild /p:Configuration=%BUILD_TYPE% DearImGui\DearImGui.csproj || goto err

echo %COLOR_WARN%Building DearImGui.OpenTK%COLOR_NORM%
MSBuild /p:Configuration=%BUILD_TYPE% DearImGui.OpenTK\DearImGui.OpenTK.csproj || goto err

echo %COLOR_WARN%Building DearImPlot%COLOR_NORM%
MSBuild /p:Configuration=%BUILD_TYPE% DearImPlot\DearImPlot.csproj || goto err

echo %COLOR_WARN%Building SampleApplication.OpenTK%COLOR_NORM%
MSBuild /p:Configuration=%BUILD_TYPE% SampleApplication.OpenTK\SampleApplication.OpenTK.csproj || goto err

echo %COLOR_WARN%Built DearImGui successfully%COLOR_NORM%

goto end

:err
echo %COLOR_WARN%Aborting, last command failed with error code %ERRORLEVEL%%COLOR_NORM%

:end
set BUILD_TYPE=
set COLOR_NORM=
set COLOR_WARN=