using System.Collections.Immutable;
using CppSharp;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Passes;

namespace im.NET.Generator.Passes;

public abstract class ImBaseGeneratorOutputPass : GeneratorOutputPass
{
    protected ImBaseGeneratorOutputPass(ImmutableSortedSet<string> namespaces)
    {
        Namespaces = namespaces;
    }

    private ImmutableSortedSet<string> Namespaces { get; }

    public override void VisitCodeGenerator(CodeGenerator generator)
    {
        if (generator is not CSharpSources sources)
        {
            return;
        }

        if (!CanApply(sources))
        {
            return;
        }

        // disable some warnings

        var header = sources.FindBlock(BlockKind.Header);

        header.Text.WriteLine(
            "#pragma warning disable CS0109 // The member 'member' does not hide an inherited member. The new keyword is not required");

        header.Text.WriteLine(
            "#pragma warning disable CS0414 // The private field 'field' is assigned but its value is never used");

        header.Text.WriteLine( // there is no generic solution for these ones, e.g. delegates, indexers
            "#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Type_or_Member'");

        // add used usings

        var usings = sources.FindBlock(BlockKind.Usings);

        var usingsText = usings.Text;

        usingsText.StringBuilder.Clear();

        foreach (var item in Namespaces)
        {
            usingsText.WriteLine($"using {item};");
        }

        usingsText.WriteLine("using ImS8  = System.SByte;");
        usingsText.WriteLine("using ImU8  = System.Byte;");
        usingsText.WriteLine("using ImS16 = System.Int16;");
        usingsText.WriteLine("using ImU16 = System.UInt16;");
        usingsText.WriteLine("using ImS32 = System.Int32;");
        usingsText.WriteLine("using ImU32 = System.UInt32;");
        usingsText.WriteLine("using ImS64 = System.Int64;");
        usingsText.WriteLine("using ImU64 = System.UInt64;");

        // fix escaped new lines in comments

        var comments = sources.FindBlocks(BlockKind.BlockComment);

        foreach (var comment in comments)
        {
            comment.Text.StringBuilder.Replace("&lt;br/&gt;", "<br/>");
        }
    }

    protected abstract bool CanApply(CSharpSources sources);
}