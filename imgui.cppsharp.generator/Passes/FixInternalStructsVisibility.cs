using CppSharp;
using CppSharp.Generators;
using CppSharp.Passes;

namespace imgui.cppsharp.generator.Passes;

internal sealed class FixInternalStructsVisibility : GeneratorOutputPass
{
    public override void VisitGeneratorOutput(GeneratorOutput output)
    {
        var blocks = output.Outputs.SelectMany(s => s.FindBlocks(BlockKind.Unknown));

        foreach (var block in blocks)
        {
            block.Text.StringBuilder.Replace("public partial struct __Internal", "internal partial struct __Internal");
        }
    }
}