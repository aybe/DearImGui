using im.NET.Generator;

namespace imgui.NET.Generator;

internal static class Program
{
    private static void Main()
    {
        ConsoleGeneratorOptions.Generate(Generate);
    }

    private static void Generate(ConsoleGeneratorOptions options)
    {
        var generator = new ConsoleGeneratorImGui(options.Architecture, options.Directory);

        generator.Run();

        ConsoleGeneratorOptions.Rewrite(@".\x86\imgui.cs", @".\x64\imgui.cs", @".\imgui.AnyCPU.g.cs");
    }
}