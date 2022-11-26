using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using im.NET.Generator;

namespace imgui.NET.Generator;

internal sealed class ConsoleGeneratorImGui : ConsoleGenerator
{
    public ConsoleGeneratorImGui(string moduleName, string? directory = null) : base(moduleName, directory)
    {
        Namespaces = GetDefaultNamespaces();

        Classes = new SortedSet<KeyValuePair<string, string>>
            {
                new("imgui", "ImGui")
            }
            .ToImmutableSortedSet();

        Aliases = GetDefaultAliases();
    }

    public override ImmutableSortedSet<Type> Aliases { get; }

    public override ImmutableSortedSet<KeyValuePair<string, string>> Classes { get; }

    public override ImmutableSortedSet<string> Namespaces { get; }

    protected override void Process(ref string text)
    {
        ProcessSymbols(ref text);

        ProcessVectorClass(ref text);

        ProcessClasses(ref text);

        base.Process(ref text);
    }

    private static void ProcessClasses(ref string text)
    {
        // the symbols class has wrong visibility and lacks partial, fix it

        text = text.Replace("internal class imgui", "partial class imgui");
    }

    private static void ProcessSymbols(ref string text)
    {
        // use our own symbol resolver

        text = text.Replace(
            "CppSharp.SymbolResolver",
            $"global::{Constants.ImGuiNamespace}.SymbolResolver"
        );

        // hide public symbols that ought to be internal

        text = Regex.Replace(text,
            @"public\s+static\s+(\w+)\s+(_(?!_)\w+)",
            @"internal static $1 $2",
            RegexOptions.Multiline
        );
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ProcessVectorClass(ref string text)
    {
        // hide ImVector namespace as internal class as it cannot be moved onto ImVector<T> because of CS7042

        text = text.Replace(
            "namespace ImVector",
            "internal static partial class ImVector"
        );

        text = text.Replace(
            ".__Symbols",
            string.Empty
        );
    }
}