using CppSharp;
using ImGuiNET.Generator.Logging;

namespace ImGuiNET.Generator;

internal static class Program
{
    private static void Main()
    {
        Directory.CreateDirectory("OLD");
        Directory.CreateDirectory("NEW");

        using (var writer = new StreamWriter(File.Create(@"OLD\output.txt")))
        using (new AggregateConsoleOut(writer))
        {
            ConsoleDriver.Run(new MyLibrary { Enhanced = false });
        }

        Console.Clear();

        using (var writer = new StreamWriter(File.Create(@"NEW\output.txt")))
        using (new AggregateConsoleOut(writer))
        {
            ConsoleDriver.Run(new MyLibrary { Enhanced = true });
        }
    }
}