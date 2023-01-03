using System.Text.RegularExpressions;
using DearGenerator;

namespace DearImPlot.Generator;

internal static class Program
{
    private static async Task Main()
    {
        await CodeGenerator.Generate(
            "ImPlot",
            Environment.CurrentDirectory,
            @"..\..\..\..\DearImPlot",
            (s, t) => new CodeGeneratorImPlot(s, t),
            Transform
        );
    }

    private static string Transform(string text)
    {
        // for whatever reason, generated 64-bit code is IntPtr in some places, fix

        text = Regex.Replace(text,
            @"(__Internal64\.PlotHistogram(?:2D)?(?:_\d+)?\(.*)(__arg(?:5|6))(.*)",
            "$1new IntPtr(&$2)$3",
            RegexOptions.Multiline
        );

        return text;
    }
}