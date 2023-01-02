using im.NET.Generator;

namespace imgui.NET.Generator;

internal static class Program
{
    private static async Task Main()
    {
        await im.NET.Generator.Generator.Generate("imgui", Environment.CurrentDirectory, (s, t) => new GeneratorImGui(s, t));
    }
}