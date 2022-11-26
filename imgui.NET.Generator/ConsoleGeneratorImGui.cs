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

    public override ImmutableSortedSet<string> Namespaces { get; }

    public override ImmutableSortedSet<KeyValuePair<string, string>> Classes { get; }

    public override ImmutableSortedSet<Type> Aliases { get; }

    protected override void Process(ref string text)
    {
        ProcessSymbols(ref text);

        ProcessVectorClass(ref text);

        ProcessEnumerations(ref text);

        base.Process(ref text);
    }

    private static void ProcessSymbols(ref string text)
    {
        const string @namespace = Constants.ImGuiNamespace;

        // merge symbols with class to remove __Symbols namespace

        text = text.Replace(
            $"}}\r\nnamespace {@namespace}.__Symbols\r\n{{\r\n    internal class imgui",
            "    public unsafe partial class imgui"
        );

        // use our own symbol resolver

        text = text.Replace(
            "CppSharp.SymbolResolver",
            $"global::{@namespace}.SymbolResolver"
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
            "public static IntPtr _EmptyString_ImGuiTextBuffer__2PADA",
            "internal static IntPtr _EmptyString_ImGuiTextBuffer__2PADA"
        );

        text = text.Replace(
            ".__Symbols",
            string.Empty
        );
    }

    private static void ProcessEnumerations(ref string text)
    {
        // enumerations default values other than zero must be cast

        text = Regex.Replace(text,
            @"(?<!//\s+DEBUG:.*)(ImGui\w+Flags)\s+(\w+)\s+=\s+(\d+)",
            @"$1 $2 = ($1)($3)"
        );
    }
}