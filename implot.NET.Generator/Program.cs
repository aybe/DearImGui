using System.Diagnostics;
using CppSharp;

namespace implot.NET.Generator;

internal static class Program
{
    public static void Main(string[] args)
    {
        if (Debugger.IsAttached) // cleanup garbage
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
        }

        var generator = new GeneratorImPlot("implot");

        var library = new ImPlotLibrary
        {
            Namespaces = generator.Namespaces
        };

        ConsoleDriver.Run(library);

        generator.Process();

        Console.WriteLine("Generation finished.");
    }
}