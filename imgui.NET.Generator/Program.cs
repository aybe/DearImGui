using CommandLine;
using im.NET.Generator;

namespace imgui.NET.Generator;

internal static class Program
{
    private static void Main(string[] args)
    {
        ConsoleGeneratorOptions.SetDefaultArgumentsIfEmpty(ref args);

        Parser
            .Default
            .ParseArguments<ConsoleGeneratorOptions>(args)
            .WithParsed(Generate);
    }

    private static void Generate(ConsoleGeneratorOptions options)
    {
        var generator = new ConsoleGeneratorImGui(options.Architecture, options.Directory);

        generator.Run();
    }
}