using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CppSharp.AST;
using CppSharp.Passes;
using im.NET.Generator;

namespace implot.NET.Generator;

[SuppressMessage("ReSharper", "RedundantIfElseBlock")]
[SuppressMessage("ReSharper", "InvertIf")]
internal sealed class ImGuiEnumPass : TranslationUnitPass
{
    private const string Indent = "    ";

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

    public GeneratorType GeneratorType { get; set; }

    public bool LogIgnoredEnumeration { get; set; }

    public bool LogIgnoredEnumerationItem { get; set; }

    public bool LogRenamedEnumerationItem { get; set; }

    public bool LogIgnoredImGuiClass { get; set; }

    public bool LogIgnoredImGuiEnumeration { get; set; }

    public bool LogIgnoredImGuiFunction { get; set; }

    private bool IgnoreIfNotImGui(Declaration declaration, bool log)
    {
        if (GeneratorType is GeneratorType.ImGui)
        {
            return false;
        }

        if (declaration.TranslationUnit.FileName is not "imgui.h")
        {
            return false;
        }

        if (log)
        {
            using (GetConsoleColorScope())
            {
                Console.WriteLine($"{declaration.TranslationUnit}\t{declaration}");
            }
        }

        declaration.ExplicitlyIgnore();

        return true;
    }

    public override bool VisitClassDecl(Class @class)
    {
        if (IgnoreIfNotImGui(@class, LogIgnoredImGuiClass))
        {
            return true;
        }

        return base.VisitClassDecl(@class);
    }

    public override bool VisitFunctionDecl(Function function)
    {
        if (IgnoreIfNotImGui(function, LogIgnoredImGuiFunction))
        {
            return true;
        }

        return base.VisitFunctionDecl(function);
    }

    public override bool VisitEnumDecl(Enumeration enumeration)
    {
        if (IgnoreIfNotImGui(enumeration, LogIgnoredImGuiEnumeration))
        {
            return true;
        }

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
        if (IgnoreIfNotImGui(item, false))
        {
            throw new InvalidOperationException(); // should never be reached
        }

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

        if (item.Name.Contains(item.Namespace.Name))
        {
            item.Name = item.Name.Replace(item.Namespace.Name, string.Empty);

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

    private static ConsoleColorScope GetConsoleColorScope(ConsoleColor? backgroundColor = ConsoleColor.Red, ConsoleColor? foregroundColor = ConsoleColor.White)
    {
        return new ConsoleColorScope(backgroundColor, foregroundColor);
    }

    private void CheckImGui(Enumeration.Item item)
    {
        if (GeneratorType is GeneratorType.ImGui)
            return;

        if (IsImGuiTranslationUnit(item))
        {
            throw new InvalidOperationException($"ImGui declaration was unexpected: {item}");
        }
    }

    private static bool IsImGuiTranslationUnit(Declaration declaration)
    {
        return declaration.TranslationUnit.FileName is "imgui.h";
    }
}