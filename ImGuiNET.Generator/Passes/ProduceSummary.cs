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

    public override bool VisitFunctionDecl(Function function)
    {
        TrySetComment(function, -1);
        return base.VisitFunctionDecl(function);
    }

    public override bool VisitEnumDecl(Enumeration @enum)
    {
        TrySetComment(@enum, -2);
        return base.VisitEnumDecl(@enum);
    }

    public override bool VisitEnumItemDecl(Enumeration.Item item)
    {
        const string note = "(NOTE: summary auto-generated from source)";

        TrySetComment(item, -1, s => s.Declaration.Name == "None" ? "None" : $"{s.Declaration.Name} {note}");
        return base.VisitEnumItemDecl(item);
    }

    public override bool VisitFieldDecl(Field field)
    {
        TrySetComment(field, -1);
        return base.VisitFieldDecl(field);
    }

    private void TrySetComment<T>(T decl, int lineOffset, Func<Summary<T>, string?>? func = null) where T : Declaration
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

        if (!TryGetComment(line, out var result))
        {
            result = func?.Invoke(new Summary<T>(decl, lines));
        }

        if (result == null)
        {
            Console.WriteLine($"Couldn't find comment for {decl.Name} @ {line1}-{line2}.");
            return;
        }

        var text = result;

        text = text.Trim(' ', '.');
        text = $"{text}.";
        text = $"{char.ToUpperInvariant(text[0])}{text[1..]}";
        text = text
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("&", "&amp;")
            .Replace("'", "&apos;")
            .Replace("\"", "&quot;");

        decl.Comment = new RawComment { BriefText = text };
    }

    private static bool TryGetComment(string text, out string result)
    {
        const string mark = "//";

        result = default!;

        var indexOf = text.IndexOf(mark, StringComparison.Ordinal);
        if (indexOf is -1)
            return false;

        var temp = text[(indexOf + mark.Length)..];

        result = temp;
        return true;
    }

    #region Nested type: Summary

    private readonly struct Summary<T> where T : Declaration
    {
        public T Declaration { get; }

        public string[] Sources { get; }

        public Summary(T declaration, string[] sources)
        {
            Declaration = declaration;
            Sources = sources;
        }
    }

    #endregion
}