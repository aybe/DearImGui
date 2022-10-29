using CppSharp;
using ImGuiNET.Generator.Logging;

namespace ImGuiNET.Generator;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length is 0)
        {
            Console.WriteLine("No arguments given, available arguments:");
            Console.WriteLine("\t-old: generate old version");
            Console.WriteLine("\t-new: generate new version");
            return;
        }

        Console.WriteLine("Code generation started.");

        if (args.Contains("-old", StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine("Generating old version.");

            Directory.CreateDirectory("OLD");

            using (var writer = new StreamWriter(File.Create(@"OLD\output.txt")))
            using (new AggregateConsoleOut(writer))
            {
                ConsoleDriver.Run(new MyLibrary { Enhanced = false });
            }
        }

        if (args.Contains("-new", StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine("Generating new version.");

            Directory.CreateDirectory("NEW");

            using (var writer = new StreamWriter(File.Create(@"NEW\output.txt")))
            using (new AggregateConsoleOut(writer))
            {
                ConsoleDriver.Run(new MyLibrary { Enhanced = true });
            }
        }

        Console.WriteLine("Code generation finished.");
    }
}