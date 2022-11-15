using System.Runtime.CompilerServices;
using CppSharp.AST;
using CppSharp.Passes;
using im.NET.Generator;

namespace implot.NET.Generator;

internal class ImGuiIgnorePass : TranslationUnitPass
{
    protected const string Indent = "    "; // Microsoft Visual Studio Debug Console sucks

    public GeneratorType GeneratorType { get; set; }

    public bool LogIgnoredEnumeration { get; set; }

    public bool LogIgnoredEnumerationItem { get; set; }

    public bool LogRenamedEnumerationItem { get; set; }

    public bool LogIgnoredImGuiClass { get; set; }

    public bool LogIgnoredImGuiEnumeration { get; set; }

    public bool LogIgnoredImGuiFunction { get; set; }

    public bool LogIgnoredImGuiTypedefDecl { get; set; } = true;

    protected bool IgnoreIfNotImGui(Declaration declaration, bool log, [CallerMemberName] string memberName = null!)
    {
        // when we're not generating for imgui, we want to ignore stuff from imgui
        
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
            using (ImGuiEnumPass.GetConsoleColorScope())
            {
                Console.WriteLine($"{ImGuiEnumPass.Indent}Ignoring imgui declaration from {memberName}: {declaration}");
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

    public override bool VisitTypedefDecl(TypedefDecl typedef)
    {
        if (IgnoreIfNotImGui(typedef, LogIgnoredImGuiTypedefDecl))
        {
            return true;
        }

        return base.VisitTypedefDecl(typedef);
    }

    protected static ConsoleColorScope GetConsoleColorScope(ConsoleColor? backgroundColor = ConsoleColor.Red, ConsoleColor? foregroundColor = ConsoleColor.White)
    {
        return new ConsoleColorScope(backgroundColor, foregroundColor);
    }
}