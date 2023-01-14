@echo off

pushd build

tar -cvf debug.zip ^
x64\Debug\imgui.dll ^
x64\Debug\imgui.pdb ^
x64\Debug\implot.dll ^
x64\Debug\implot.pdb ^
x86\Debug\imgui.dll ^
x86\Debug\imgui.pdb ^
x86\Debug\implot.dll ^
x86\Debug\implot.pdb

popd