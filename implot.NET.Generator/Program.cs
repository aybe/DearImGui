using im.NET.Generator;

namespace implot.NET.Generator;

internal static class Program
{
    public static void Main(string[] args)
    {
        var generator = new ConsoleGeneratorImPlot("implot");

        var library = new ImPlotLibrary
        {
            Namespaces = generator.Namespaces
        };

        ConsoleGenerator.Run(library, generator);
    }
}