using System.Text.RegularExpressions;
using im.NET.Generator;

namespace implot.NET.Generator;

internal static class Program
{
    private static async Task Main()
    {
        await im.NET.Generator.CodeGenerator.Generate("implot", Environment.CurrentDirectory, (s, t) => new CodeGeneratorImPlot(s, t), Transform);
    }

    private static string Transform(string text)
    {
        text = Regex.Replace(text,
            @"(__Internal64\.PlotHistogram(?:2D)?(?:_\d+)?\(.*)(__arg(?:5|6))(.*)",
            "$1new IntPtr(&$2)$3",
            RegexOptions.Multiline
        );

        return text;
    }
}