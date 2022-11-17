using System.Collections.Immutable;
using System.Text.RegularExpressions;
using im.NET.Generator;

// ReSharper disable StringLiteralTypo

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
        const string @namespace = Constants.ImGuiNamespace;

        // merge symbols with class to remove __Symbols namespace

        text = text.Replace(
            $"}}\r\nnamespace {@namespace}.__Symbols\r\n{{\r\n    internal class imgui",
            "    public unsafe partial class imgui"
        );

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

        // use our own symbol resolver

        text = text.Replace(
            "CppSharp.SymbolResolver",
            $"{@namespace}.SymbolResolver"
        );

        // add some inherit doc

        text = Regex.Replace(
            text,
            @"^(\s+)(public void Dispose\(\))",
            @"$1/// <inheritdoc />$1$2",
            RegexOptions.Multiline
        );

        text = Regex.Replace(
            text,
            @"^(\s+)(~\w+\(\))",
            @"$1/// <inheritdoc />$1$2",
            RegexOptions.Multiline
        );

        base.Process(ref text);
    }
}