using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using im.NET.Generator;

namespace implot.NET.Generator;

internal sealed class GeneratorImPlot : GeneratorBase
{
    public GeneratorImPlot(string moduleName, string? directory = null) : base(moduleName, directory)
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
        
        Aliases = new SortedSet<Type>(TypeNameComparer.Instance)
            {
                typeof(CallingConvention),
                typeof(IntPtr)
            }
            .ToImmutableSortedSet(TypeNameComparer.Instance);
    }

    public override ImmutableSortedSet<string> Namespaces { get; }

    public override ImmutableSortedSet<KeyValuePair<string, string>> Classes { get; }

    public override ImmutableSortedSet<Type> Aliases { get; }

    protected override void Process(ref string text)
    {
        ProcessClasses(ref text, Classes);

        ProcessVectors(ref text);

        ProcessQualifiers(ref text, Namespaces);

        ProcessAliases(ref text, Aliases);

        ProcessGenericMethods(ref text);
    }

    private static void ProcessAliases(ref string input, ImmutableSortedSet<Type> aliases)
    {
        foreach (var item in aliases)
        {
            input = input.Replace($"__{item.Name}", item.Name);
        }
    }

    private static void ProcessClasses(ref string input, ImmutableSortedSet<KeyValuePair<string, string>> classes)
    {
        foreach (var item in classes)
        {
            input = input.Replace($"class {item.Key}", $"class {item.Value}");
        }
    }

    private static void ProcessGenericMethods(ref string input)
    {
        // add generic type parameter with unmanaged constraint

        // TODO see if that can't be done upstream anyhow
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

    private static void ProcessQualifiers(ref string input, ImmutableSortedSet<string> namespaces)
    {
        foreach (var item in namespaces.Reverse())
        {
            input = input.Replace($"global::{item}.", string.Empty);
        }
    }

    private static void ProcessVectors(ref string input)
    {
        input = input.Replace(
            "new global::implot.NET.ImVec2.__Internal()",
            "new global::System.Numerics.Vector2()"
        );

        input = input.Replace(
            "new global::implot.NET.ImVec4.__Internal()",
            "new global::System.Numerics.Vector4()"
        );
    }
}