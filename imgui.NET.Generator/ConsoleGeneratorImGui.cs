using System.Collections.Immutable;
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

    protected override void Process(ref string input)
    {
        base.Process(ref input);

        // the symbols class has wrong visibility and lacks partial, fix it

        input = input.Replace("internal class ImGui", "partial class ImGui");

        // hide ImVector namespace as internal class as it cannot be moved onto ImVector<T> because of CS7042

        input = input.Replace(
            "namespace ImVector",
            "internal static partial class ImVector"
        );

        input = input.Replace(
            ".__Symbols",
            string.Empty
        );
    }

    protected override void ProcessSymbols(ref string text)
    {
        // use our own symbol resolver

        text = text.Replace(
            "CppSharp.SymbolResolver",
            $"global::{Constants.ImGuiNamespace}.SymbolResolver"
        );

        base.ProcessSymbols(ref text);
    }
}