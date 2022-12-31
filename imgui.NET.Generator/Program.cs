using im.NET.Generator;

namespace imgui.NET.Generator;

internal static class Program
{
    private static void Main()
    {
        var options = new ConsoleGeneratorOutputs(@".\x86\imgui.cs", @".\x64\imgui.cs", @".\imgui.AnyCPU.g.cs");

        ConsoleGenerator.Generate((s, t) => new ConsoleGeneratorImGui(s, t), options);
    }
}