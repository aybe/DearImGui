using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using im.NET.Generator;

namespace implot.NET.Generator;

internal sealed class ConsoleGeneratorImPlot : ConsoleGenerator
{
    public ConsoleGeneratorImPlot(string moduleName, string? directory = null) : base(moduleName, directory)
    {
        Namespaces = new SortedSet<string>
            {
                "imgui.NET",
                "implot.NET",
                "System",
                "System.Collections.Concurrent",
                "System.Numerics",
                "System.Runtime.CompilerServices",
                "System.Runtime.InteropServices",
                "System.Security",
                "System.Text"
            }
            .ToImmutableSortedSet();

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

    protected override void Process(ref string text)
    {
        ProcessVectors(ref text);

        ProcessGenericMethods(ref text);

        base.Process(ref text);
    }

    private static void ProcessGenericMethods(ref string input)
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
    }

    private static void ProcessVectors(ref string input)
    {
        input = Regex.Replace(input,
            @"new global::implot\.NET\.ImVec(\d)\.__Internal\(\)",
            @"new global::System.Numerics.Vector$1()",
            RegexOptions.Multiline
        );
    }
}