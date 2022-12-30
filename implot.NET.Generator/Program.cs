using System.Text.RegularExpressions;
using im.NET.Generator;

namespace implot.NET.Generator;

internal static class Program
{
    private static void Main()
    {
        ConsoleGeneratorOptions.Generate(Generate);
    }

    private static void Generate(ConsoleGeneratorOptions options)
    {
        var generator = new ConsoleGeneratorImPlot(options.Architecture, options.Directory);

        generator.Run();

        const string csPathAnyCpu = @".\implot.AnyCPU.g.cs";

        ConsoleGeneratorOptions.Rewrite(@".\x86\implot.cs", @".\x64\implot.cs", csPathAnyCpu);

        var text = File.ReadAllText(csPathAnyCpu);

        text = Regex.Replace(text,
            @"(__Internal64\.PlotHistogram(?:2D)?(?:_\d+)?\(.*)(__arg(?:5|6))(.*)",
            "$1new IntPtr(&$2)$3",
            RegexOptions.Multiline
        );

        File.WriteAllText(csPathAnyCpu, text);
    }
}