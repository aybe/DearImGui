using CppSharp;
using CppSharp.Generators;
using CppSharp.Passes;

namespace ImGuiNET.Generator.Passes;

internal sealed class MyRenameOutputPass : GeneratorOutputPass
{
    public MyRenameOutputPass(params KeyValuePair<string, string>[] pairs)
    {
        Pairs = pairs;
    }

    private KeyValuePair<string, string>[] Pairs { get; }

    public override void VisitGeneratorOutput(GeneratorOutput output)
    {
        var blocks = output.Outputs.SelectMany(i => i.FindBlocks(BlockKind.Unknown));

        foreach (var block in blocks)
        {
            foreach (var pair in Pairs)
            {
                block.Text.StringBuilder.Replace(pair.Key, pair.Value);
            }
        }
    }
}