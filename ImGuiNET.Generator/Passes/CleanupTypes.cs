using CppSharp;
using CppSharp.Generators;
using CppSharp.Passes;

namespace ImGuiNET.Generator.Passes;

internal sealed class CleanupTypes : GeneratorOutputPass
{
    public override void VisitGeneratorOutput(GeneratorOutput output)
    {
        var blocks = output.Outputs.SelectMany(s => s.FindBlocks(BlockKind.Unknown));

        foreach (var block in blocks)
        {
            var builder = block.Text.StringBuilder;

            builder.Replace("class imgui", "class ImGui");

            // BUG still internal ImDrawListSharedData.__Internal
            // BUG still internal ImFontBuilderIO.__Internal
            // BUG still internal ImGuiContext.__Internal
            // BUG still internal ImVector.__Internal

            builder.Replace("public partial struct __Internal", "internal partial struct __Internal");
        }
    }
}