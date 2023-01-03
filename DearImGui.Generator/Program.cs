using DearGenerator;

namespace DearImGui.Generator;

internal static class Program
{
    private static async Task Main()
    {
        await CodeGenerator.Generate(
            "ImGui",
            Environment.CurrentDirectory,
            @"..\..\..\..\DearImGui",
            (s, t) => new CodeGeneratorImGui(s, t)
        );
    }
}