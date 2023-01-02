using im.NET.Generator;

namespace imgui.NET.Generator;

internal static class Program
{
    private static async Task Main()
    {
        await CodeGenerator.Generate(
            "ImGui",
            Environment.CurrentDirectory,
            @"..\..\..\..\imgui.NET",
            (s, t) => new CodeGeneratorImGui(s, t)
        );
    }
}