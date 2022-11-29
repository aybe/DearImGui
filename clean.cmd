@echo off

if not defined VSCMD_VER (
    echo This script must be run inside Developer Command Prompt!
    goto :eof
)

MSBuild /t:Clean /p:Configuration=Debug /p:Platform="Any CPU"
MSBuild /t:Clean /p:Configuration=Debug /p:Platform=x86
MSBuild /t:Clean /p:Configuration=Debug /p:Platform=x64
MSBuild /t:Clean /p:Configuration=Release /p:Platform="Any CPU"
MSBuild /t:Clean /p:Configuration=Release /p:Platform=x86
MSBuild /t:Clean /p:Configuration=Release /p:Platform=x64
