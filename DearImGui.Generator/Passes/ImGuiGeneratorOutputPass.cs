using System.Collections.Immutable;
using CppSharp.Generators.CSharp;
using DearGenerator.Passes;

namespace DearImGui.Generator.Passes;

internal sealed class ImGuiGeneratorOutputPass : ImBaseGeneratorOutputPass
{
    public ImGuiGeneratorOutputPass(ImmutableSortedSet<string> namespaces)
        : base(namespaces)
    {
    }

    protected override bool CanApply(CSharpSources sources)
    {
        return sources.Module.LibraryName is "imgui";
    }
}