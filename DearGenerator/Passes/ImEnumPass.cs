using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using CppSharp.AST;
using JetBrains.Annotations;

namespace DearGenerator.Passes;

[SuppressMessage("ReSharper", "RedundantIfElseBlock")]
[SuppressMessage("ReSharper", "InvertIf")]
public sealed class ImEnumPass : ImBaseTranslationUnitPass
{
    [PublicAPI]
    public bool LogIgnoredEnumeration { get; set; } = true;

    [PublicAPI]
    public bool LogIgnoredEnumerationItem { get; set; } = true;

    [PublicAPI]
    public bool LogRenamedEnumerationItem { get; set; } = true;

    private static IEnumerable<string> IgnoredSuffixes { get; } =
        new ReadOnlyCollection<string>(
            new[]
            {
                "_BEGIN",
                "_END",
                "_COUNT",
                "_SIZE",
                "_OFFSET"
            });

    public override bool VisitEnumDecl(Enumeration enumeration)
    {
        // ignore enumerations whose name contains 'Obsolete'

        if (enumeration.Name.Contains("Obsolete"))
        {
            enumeration.ExplicitlyIgnore();

            if (LogIgnoredEnumeration)
            {
                using (GetConsoleColorScope())
                {
                    Console.WriteLine($"{Indent}Ignored enumeration {enumeration}");
                }
            }

            return true;
        }

        return base.VisitEnumDecl(enumeration);
    }

    public override bool VisitEnumItemDecl(Enumeration.Item item)
    {
        // ignore enumerations items with some suffixes, these are only useful when using C++

        var suffix = IgnoredSuffixes.FirstOrDefault(s => item.Name.EndsWith(s, StringComparison.Ordinal));

        if (suffix != null)
        {
            item.ExplicitlyIgnore();

            if (LogIgnoredEnumerationItem)
            {
                using (GetConsoleColorScope())
                {
                    Console.WriteLine($"{Indent}Ignored enumeration item {item.Name} with suffix {suffix}");
                }
            }

            return true;
        }

        // remove enumeration from enumeration item name, e.g. ImGuiCond_Always -> Always

        if (item.Name.Contains(item.Namespace.Name))
        {
            item.Name = item.Name.Replace(item.Namespace.Name, string.Empty);

            // prefix enumeration name with a 'D' when it starts with a digit, e.g. 1 -> D1

            var match = Regex.Match(item.Name, @"^_(\d+)$");

            if (match.Success)
            {
                item.Name = $"D{match.Groups[1].Value}";
            }

            if (LogRenamedEnumerationItem)
            {
                using (GetConsoleColorScope())
                {
                    Console.WriteLine($"{Indent}Renamed enumeration item {item.OriginalName} to {item.Name}");
                }
            }

            return true;
        }

        return base.VisitEnumItemDecl(item);
    }
}