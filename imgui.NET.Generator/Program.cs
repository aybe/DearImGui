using im.NET.Generator;

namespace imgui.NET.Generator;

internal static class Program
{
    private static async Task Main()
    {
        await im.NET.Generator.CodeGenerator.Generate("imgui", Environment.CurrentDirectory, (s, t) => new CodeGeneratorImGui(s, t));
    }
}