
# imgui.NET

This is [imgui](https://github.com/ocornut/imgui) for .NET, generated using [CppSharp](https://github.com/mono/CppSharp).


# Getting started

## Development

### Configure nuget package source

The generator and library projects use the [most recent packages of CppSharp](https://github.com/orgs/mono/packages?repo_name=CppSharp).

In order to restore them, you will need to generate [your own personal access token](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry).

Next, [add a package source](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio) to `https://nuget.pkg.github.com/mono/index.json`.

From there, the packages will be properly restored when building the solution.

### Generate the bindings

Batch build all of the configurations available for **imgui** project.

Generate all the T4 templates for **imgui.NET.Generator** project.

Finally, run the generator project with arguments `-new -cln`.

From there, the **imgui.NET** project can be built successfully.

### Test the generated bindings

Take a look at the project **imgui.NET.OpenTK.Test** in solution.
