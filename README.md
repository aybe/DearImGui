
# DearImGui

This is [imgui](https://github.com/ocornut/imgui) and [implot](https://github.com/epezent/implot) bindings for .NET, including a controller for [OpenTK](https://github.com/opentk/opentk).

## Features

- Original documentation, when present, is available through IntelliSense
- Friendly types when possible: Span\<T>, vectors from System.Numerics
- Close to zero allocations, the garbage collector will be your best friend!

## Getting started

Windows, AnyCPU, .NET 6.0, OpenGL 4.5:

[![Nuget](https://img.shields.io/nuget/v/DearImGui?label=DearImGui)](https://www.nuget.org/packages/DearImGui)
[![Nuget](https://img.shields.io/nuget/v/DearImGui.OpenTK?label=DearImGui.OpenTK)](https://www.nuget.org/packages/DearImGui.OpenTK)
[![Nuget](https://img.shields.io/nuget/v/DearImPlot?label=DearImPlot)](https://www.nuget.org/packages/DearImPlot)

You will also need [Microsoft Visual C++ Redistributable latest supported downloads](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170):

- https://aka.ms/vs/17/release/vc_redist.x86.exe
- https://aka.ms/vs/17/release/vc_redist.x64.exe

Should you need debug builds of native DLLs, you will find them in the [Releases](https://github.com/aybe/DearImGui/releases) tab.

## Getting started (development)

### General notes

Unlike similar projects, this one is generated directly against sources.

The pros of this approach are, for instance:

- documentation (when there is) for pretty much every member out there
- default values for optional parameters, i.e. 'vanilla' imgui interface

In short, we can closely mimic the interfaces of the original projects.

But there are cons as well, for instance:

- the generated code is better but the generators are more complex
- DLL exports with decorated names as these differ in 32-bit VS 64-bit

The former was solved by spending quite some time polishing the generators.

The latter was solved using [Roslyn](https://github.com/dotnet/roslyn), rewriting the entire output to be AnyCPU.

### Cloning

The repository has submodules, don't forget to clone them in the process:

`git clone --recurse-submodules https://github.com/aybe/DearImGui.git`

### Building

Restoring [CppSharp](https://github.com/orgs/mono/packages?repo_name=CppSharp) from GitHub's NuGet registry requires a [personal access token](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry).

In order for the projects in *Managed* folder to build, there are implicit dependencies:

- projects in *Generated* folder shall be started at least once
- projects in *Native* folder shall be batch built at least once

These dependencies can be pre-built by invoking `dearimgui-build.cmd $(Configuration)`.

### Extending

It should be possible to support other libraries such as [imguizmo](https://github.com/CedricGuillemet/ImGuizmo) and [imnodes](https://github.com/Nelarius/imnodes).

Take a look at how [implot](https://github.com/aybe/DearImGui/tree/develop/DearImPlot.Generator) is generated and the [shared code](https://github.com/aybe/DearImGui/tree/develop/DearGenerator) used by generators.

However, few things may prove to be challenging due to how CppSharp works.

When bindings are generated, there is a version history for easily diff'ing them.

### Known issues

**Generator can't write output file:**

Occasionally, you may encounter a similiar exception while generating:

> 4>Unhandled exception. System.IO.IOException: The process cannot access the file 'C:\\...\\DearImGui\\DearImGui.Generator\\bin\\Release\\net6.0\\imgui.cs' because it is being used by another process.

Something has outstanding handles on that file, try to generate again.

# Credits

https://github.com/ocornut/imgui

https://github.com/epezent/implot

https://github.com/mono/CppSharp

https://github.com/opentk/opentk

https://github.com/dotnet/pinvoke

https://github.com/dotnet/roslyn

https://github.com/dotnet/sourcelink

https://fonts.google.com/specimen/Roboto

https://www.jetbrains.com/resharper/
