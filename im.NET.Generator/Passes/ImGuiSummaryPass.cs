using System.Text.RegularExpressions;
using CppSharp.AST;
using CppSharp.Passes;

namespace im.NET.Generator.Passes;

public sealed class ImGuiSummaryPass : TranslationUnitPass
{
    private readonly Dictionary<TranslationUnit, string[]> Dictionary = new();

    public override bool VisitDeclaration(Declaration decl)
    {
        TryComment(decl, decl is Enumeration.Item ? EnumNone : null);
        return base.VisitDeclaration(decl);
    }

    private static string? EnumNone(Declaration declaration)
    {
        if (declaration is not Enumeration.Item item)
            return null;

        const string none = "None";

        if (item.Name == none)
        {
            return none;
        }

        const string key = "ImGuiKey_";

        if (item.Namespace.Name == key && item.DebugText.Contains("//") is false)
        {
            return item.Name.Replace(key, string.Empty);
        }

        return null;
    }

    private string[] GetSourceLines<T>(T decl) where T : Declaration
    {
        var unit = decl.TranslationUnit;

        if (!Dictionary.ContainsKey(unit))
        {
            Dictionary[unit] = File.ReadAllLines(unit.FilePath);
        }

        var lines = Dictionary[unit];

        return lines;
    }

    private static bool Ignore(Declaration declaration)
    {
        if (declaration.Namespace == null)
        {
            return true; // "T" ["TypeTemplateParameter"]
        }

        var unit = declaration.TranslationUnit;

        if (unit is null)
        {
            return true;
        }

        if (unit.FileName != "imgui.h")
        {
            return true;
        }

        if (declaration.Comment != null)
        {
            return true;
        }

        if (declaration is TypedefDecl && declaration.DebugText.IndexOf('(') is -1)
        {
            return true; // only callbacks otherwise we'll introduce tons of CS1587
        }

        return false;
    }

    private void TryComment(Declaration declaration, Func<Declaration, string?>? getter = null)
    {
        if (Ignore(declaration))
        {
            return;
        }

        var start = declaration.LineNumberStart;

        if (start is -1)
        {
            return;
        }

        var stack = new Stack<string>();

        stack.Push($@"{Constants.ImGuiHeaderUrl}#L{start}");

        TryFindComment(declaration, stack, getter);

        var value = string.Join("<br/>", stack.Select(Normalize));

        declaration.Comment = new RawComment
        {
            BriefText = value
        };
    }

    private static string Normalize(string value)
    {
        value = value.Trim(' ', '.');

        if (value.EndsWith('!') is false)
        {
            value = $"{value}.";
        }

        value = Regex.Replace(value, @"\s{2,}", @", ");

        if (value.StartsWith(Constants.ImGuiHeaderUrl) is false)
        {
            value = $"{char.ToUpperInvariant(value[0])}{value[1..]}";
        }

        value = value.Replace(@"&", @"&amp;").Replace(@"<", @"&lt;").Replace(@">", @"&gt;");

        return value;
    }

    private static bool TryFindCommentAbove(IReadOnlyList<string> lines, int index, Stack<string> stack)
    {
        var count = stack.Count;

        do
        {
            var input = lines[index];

            if (Regex.IsMatch(input, @"^\s*$")) // empty line
            {
                break;
            }

            var match = Regex.Match(input, @"^\s*//\s?(.*)$"); // comment

            if (match.Success is false)
            {
                continue;
            }

            var value = match.Groups[1].Value;

            stack.Push(value);
        } while (--index > 0);

        return stack.Count != count;
    }

    private static bool TryFindCommentRight(IReadOnlyList<string> lines, int index, Stack<string> stack)
    {
        var input = lines[index];

        if (input.Contains('/') is false)
        {
            return false;
        }

        var match = Regex.Match(input, @"[^/]+$"); // anything after last slash

        if (match.Success is false)
        {
            return false;
        }

        var value = match.Value;

        stack.Push(value);

        return true;
    }

    private bool TryFindComment(Declaration declaration, Stack<string> stack, Func<Declaration, string?>? getter)
    {
        var lines = GetSourceLines(declaration);

        var index = declaration.LineNumberStart - 1;

        if (TryFindCommentRight(lines, index, stack))
        {
            return true;
        }

        var value = getter?.Invoke(declaration);

        if (value != null)
        {
            stack.Push(value);
            return true;
        }

        if (TryFindCommentAbove(lines, index, stack))
        {
            return true;
        }

        return false;
    }
}