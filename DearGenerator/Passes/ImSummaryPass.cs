using System.Collections.Immutable;
using System.Text.RegularExpressions;
using CppSharp.AST;
using CppSharp.Passes;
using DearGenerator.Extensions;

namespace DearGenerator.Passes;

public abstract class ImSummaryPass : TranslationUnitPass
{
    private readonly Dictionary<string, string[]> Dictionary = new();

    protected abstract ImmutableSortedSet<string> HeaderNames { get; }

    protected abstract string HeaderUrl { get; }

    public override bool VisitDeclaration(Declaration declaration)
    {
        TryGetComments(declaration, declaration is Enumeration.Item ? EnumSummary : null);

        return base.VisitDeclaration(declaration);
    }

    private static string? EnumSummary(Declaration declaration)
    {
        if (declaration is not Enumeration.Item item)
        {
            return null;
        }

        // we do not compare with zero because some enum values don't mean 'None'

        if (string.Equals(item.Name, "None", StringComparison.OrdinalIgnoreCase))
        {
            return "Use the default behavior.";
        }

        // for keys enumerations, the comment will be enum suffix

        const string key = "ImGuiKey_";

        if (item.Namespace.Name != key)
        {
            return null;
        }

        if (item.DebugText.Contains("//"))
        {
            return null;
        }

        var summary = item.Name.Replace(key, string.Empty);

        summary = $"The key '{summary}'.";

        return summary;
    }

    private void TryGetComments(Declaration declaration, Func<Declaration, string?>? summary = null)
    {
        // ignore irrelevant and system declarations

        if (Ignore(declaration))
        {
            return;
        }

        var start = declaration.LineNumberStart;

        if (start is -1)
        {
            return;
        }

        // try grab comments, remove empty lines, always add a GitHub link to header @ line

        var comments = new List<string>();

        if (!TryGetComments(declaration, summary, comments, ref start))
        {
            using (new ConsoleColorScope(foregroundColor: ConsoleColor.Red))
            {
                var text = Regex.Replace(declaration.ToString(), @"\t|\s+", " ");
                Console.WriteLine($"Couldn't find comment for declaration at line {start}: {text}.");
            }
        }

        comments.RemoveAll(string.IsNullOrWhiteSpace);

        comments.Add($@"{HeaderUrl}#L{start}");

        declaration.Comment = new RawComment
        {
            BriefText = string.Join("<br/>", comments.Select(Normalize))
        };
    }

    private bool TryGetComments(Declaration declaration, Func<Declaration, string?>? summary, IList<string> comments, ref int start)
    {
        // try find the comments either from right, callback, or above

        var lines = GetSourceLines(declaration.TranslationUnit.FilePath);

        var index = declaration.LineNumberStart - 1;

        // if comment on right is our special doc comment, update refs

        var value = lines[index];

        var match = Regex.Match(value, @"//\s+" + Constants.ImGuiNamespace + @"\s+@\s+""(?<header>[\w\:\.\\]+)""\s+@\s+(?<index1>\d+)\|(?<index2>\d+)$");

        if (match.Success)
        {
            var header = match.Groups["header"].Value;
            var index1 = match.Groups["index1"].Value;
            var index2 = match.Groups["index2"].Value;
            var unit = ASTContext.TranslationUnits.Single(s => string.Equals(s.FilePath, header, StringComparison.OrdinalIgnoreCase));
            lines = GetSourceLines(unit.FilePath);
            index = int.Parse(index1) - 1;
            start = int.Parse(index2);
        }

        // if declaration is an enum 'None', defer to callback instead

        if (declaration is not Enumeration.Item { Name: "None" })
        {
            if (TryGetCommentsRight(lines, index, comments))
            {
                return true;
            }
        }

        var callback = summary?.Invoke(declaration);

        if (callback != null)
        {
            comments.Add(callback);
            return true;
        }

        var above = TryGetCommentsAbove(lines, index, comments);

        return above;
    }

    private static bool TryGetCommentsAbove(IReadOnlyList<string> lines, int index, IList<string> comments)
    {
        // grab consecutive comments immediately preceding declaration

        var count = comments.Count;

        while (--index >= 0)
        {
            var input = lines[index];

            if (comments.Count == count)
            {
                if (Regex.IsMatch(input, @"^\s*$"))
                {
                    continue; // skip empty lines before declaration
                }
            }

            var match = Regex.Match(input, @"(?<=^\s*//\s).*");

            if (match.Success)
            {
                comments.Insert(0, match.Value);
            }
            else
            {
                break;
            }
        }

        return comments.Count != count;
    }

    private static bool TryGetCommentsRight(IReadOnlyList<string> lines, int index, ICollection<string> comments)
    {
        var count = comments.Count;

        var input = lines[index];

        // match multiple comments after declaration

        var matches = Regex.Matches(input, @"//.*?(?=//|$)");

        // split multiple spaces as different lines

        foreach (var match in matches.Cast<Match>())
        {
            var split = Regex.Split(match.Value, @"\s{2,}");

            foreach (var value in split)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                var item = value.Trim('/', ' ');

                comments.Add(item);
            }
        }

        return comments.Count != count;
    }

    private string[] GetSourceLines(string header)
    {
        if (!Dictionary.ContainsKey(header))
        {
            Dictionary[header] = File.ReadAllLines(header);
        }

        var lines = Dictionary[header];

        return lines;
    }

    private bool Ignore(Declaration declaration)
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

        if (HeaderNames.All(s => !string.Equals(s, unit.FileName, StringComparison.Ordinal)))
        {
            return true;
        }

        if (declaration.Comment != null)
        {
            return true;
        }

        if (declaration is not TypedefDecl)
        {
            return false;
        }

        // only callbacks otherwise we'll introduce tons of CS1587

        var ignore = declaration.DebugText.IndexOf('(') is -1;

        return ignore;
    }

    private string Normalize(string value)
    {
        // prepend period if necessary

        value = value.Trim(' ', '.');

        if (value.EndsWith('!') is false)
        {
            value = $"{value}.";
        }

        // replace multiple spaces by a comma

        value = Regex.Replace(value, @"\s{2,}", @", ");

        // capitalize first letter

        if (value.StartsWith(HeaderUrl) is false)
        {
            value = $"{char.ToUpperInvariant(value[0])}{value[1..]}";
        }

        // XML-escape the stuff

        value = value.Replace(@"&", @"&amp;").Replace(@"<", @"&lt;").Replace(@">", @"&gt;");

        return value;
    }
}