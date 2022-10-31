using CppSharp.AST;
using CppSharp.Passes;

namespace ImGuiNET.Generator.Passes;

internal sealed class ProduceSummary : TranslationUnitPass
{
    private readonly Dictionary<TranslationUnit, string[]> Dictionary = new();

    public override bool VisitMethodDecl(Method method)
    {
        TrySetComment(method, -1);
        return base.VisitMethodDecl(method);
    }

    public override bool VisitEnumDecl(Enumeration @enum)
    {
        TrySetComment(@enum, -2);
        return base.VisitEnumDecl(@enum);
    }

    public override bool VisitEnumItemDecl(Enumeration.Item item)
    {
        TrySetComment(item, -1);
        return base.VisitEnumItemDecl(item);
    }

    private void TrySetComment(Declaration decl, int lineOffset)
    {
        var line1 = decl.LineNumberStart;
        var line2 = decl.LineNumberEnd;

        if (line1 != line2)
        {
            Console.WriteLine($"Warning: commenting for multi-line declaration may fail: {decl.Name} @ {line1}-{line2}.");
        }

        var unit = decl.TranslationUnit;

        if (!Dictionary.ContainsKey(unit))
        {
            Dictionary[unit] = File.ReadAllLines(unit.FilePath);
        }

        var lines = Dictionary[unit];

        var index = line1 + lineOffset;

        if (index < 0 || index >= lines.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        }

        var line = lines[index];

        if (!TryGetComment(line, out var comment))
        {
            Console.WriteLine($"Couldn't find comment for {decl.Name} @ {line1}-{line2}.");
            return;
        }

        decl.Comment = comment;
    }

    private static bool TryGetComment(string text, out RawComment comment)
    {
        const string mark = "//";

        comment = default!;

        var indexOf = text.IndexOf(mark, StringComparison.Ordinal);
        if (indexOf is -1)
            return false;

        var temp = text[(indexOf + mark.Length)..];
        temp = temp.Trim(' ', '.');
        temp = $"{temp}.";
        temp = temp
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("&", "&amp;")
            .Replace("'", "&apos;")
            .Replace("\"", "&quot;");

        comment = new RawComment { BriefText = temp };
        return true;
    }
}