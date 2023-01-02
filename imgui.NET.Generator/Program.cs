using im.NET.Generator;

namespace imgui.NET.Generator;

internal static class Program
{
    private static async Task Main()
    {
        await ConsoleGenerator.Generate("imgui", Environment.CurrentDirectory, (s, t) => new ConsoleGeneratorImGui(s, t));
    }
}