using System.Collections.Immutable;
using CppSharp.Generators.CSharp;
using DearGenerator.Passes;

namespace implot.NET.Generator.Passes;

internal sealed class ImPlotGeneratorOutputPass : ImBaseGeneratorOutputPass
{
    public ImPlotGeneratorOutputPass(ImmutableSortedSet<string> namespaces)
        : base(namespaces)
    {
    }

    protected override bool CanApply(CSharpSources sources)
    {
        return sources.Module.LibraryName is "implot";
    }
}