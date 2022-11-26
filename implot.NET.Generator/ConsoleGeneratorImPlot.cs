using System.Collections.Immutable;
using System.Text.RegularExpressions;
using im.NET.Generator;

namespace implot.NET.Generator;

internal sealed class ConsoleGeneratorImPlot : ConsoleGenerator
{
    public ConsoleGeneratorImPlot(string moduleName, string? directory = null) : base(moduleName, directory)
    {
        Namespaces = GetDefaultNamespaces().Add("implot.NET");

        Classes = new SortedSet<KeyValuePair<string, string>>
            {
                new("implot", "ImPlot")
            }
            .ToImmutableSortedSet();

        Aliases = GetDefaultAliases();
    }

    public override ImmutableSortedSet<string> Namespaces { get; }

    public override ImmutableSortedSet<KeyValuePair<string, string>> Classes { get; }

    public override ImmutableSortedSet<Type> Aliases { get; }

    protected override void Process(ref string input)
    {
        base.Process(ref input);

        // fix enumeration syntax for negative values

        input = Regex.Replace(input,
            @"(?<!(?:static|///\s*<summary>).*)(Colormap.*)(-1)",
            "$1(ImPlotColormap)($2)",
            RegexOptions.Multiline
        );

        // fix this wrong reference to internal pointer

        input = input.Replace(
            "ImPlotPoint.__Internal ImPlotGetter",
            "ImPlotPoint ImPlotGetter"
        );
    }

    protected override void ProcessGenericMethods(ref string input)
    {
        // add generic type parameter with unmanaged constraint

        input = Regex.Replace(input,
            @"^(\s+public\s+static\s+\w+\s+\w+)(\(.*T\s+\w+.*\))",
            @"$1<T>$2 where T : unmanaged",
            RegexOptions.Multiline
        );

        // set generic type parameters as arrays

        input = Regex.Replace(input,
            @"(,\s+T)(\s+\w+)(?=.*where\s+T\s+:\s+unmanaged\s*$)",
            @"$1[]$2",
            RegexOptions.Multiline
        );

        // sizeof(T) must be compile-time constant, compute locally

        input = Regex.Replace(input,
            @"(?<!extern.*)int\s+stride\s+=\s+sizeof\(T\)",
            "int? stride = null",
            RegexOptions.Multiline
        );

        input = Regex.Replace(input,
            @"(?<!static.*)stride",
            "stride ?? Unsafe.SizeOf<T>()",
            RegexOptions.Multiline
        );

        base.ProcessGenericMethods(ref input);
    }
}