using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CppSharp.AST;
using im.NET.Generator;

namespace implot.NET.Generator;

[SuppressMessage("ReSharper", "RedundantIfElseBlock")]
[SuppressMessage("ReSharper", "InvertIf")]
internal sealed class ImGuiEnumPass : ImPlotBasePass
{
    public ImGuiEnumPass(GeneratorType generatorType) : base(generatorType)
    {
    }

    public bool LogIgnoredEnumeration { get; set; }

    public bool LogIgnoredEnumerationItem { get; set; }

    public bool LogRenamedEnumerationItem { get; set; }

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
        // ignore enumerations items with some suffixes, these are only relevant for C++
        
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

            // prefix enumeration name with a 'D' when it starts with a digit, e.g. ImGuiKey_1 -> D1

            if (char.IsDigit(item.Name[0]))
            {
                item.Name = $"D{item.Name}";
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