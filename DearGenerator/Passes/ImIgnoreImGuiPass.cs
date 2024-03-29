﻿using System.Runtime.CompilerServices;
using CppSharp.AST;
using JetBrains.Annotations;

namespace DearGenerator.Passes;

public sealed class ImIgnoreImGuiPass : ImBaseTranslationUnitPass
{
    [PublicAPI]
    public bool LogIgnoredImGuiClass { get; set; } = true;

    [PublicAPI]
    public bool LogIgnoredImGuiEnumeration { get; set; } = true;

    [PublicAPI]
    public bool LogIgnoredImGuiEnumerationItem { get; set; } = true;

    [PublicAPI]
    public bool LogIgnoredImGuiFunction { get; set; } = true;

    [PublicAPI]
    public bool LogIgnoredImGuiTypedefDecl { get; set; } = true;

    private static bool IgnoreIfNotImGui(Declaration declaration, bool log, [CallerMemberName] string memberName = null!)
    {
        // when we're not generating for imgui, we want to ignore stuff from imgui

        if (declaration.TranslationUnit.FileName is not "imgui.h")
        {
            return false;
        }

        if (log)
        {
            using (GetConsoleColorScope())
            {
                Console.WriteLine($"{Indent}Ignoring imgui declaration from {memberName}: {declaration}");
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

    public override bool VisitEnumDecl(Enumeration enumeration)
    {
        if (IgnoreIfNotImGui(enumeration, LogIgnoredImGuiEnumeration))
        {
            return true;
        }

        return base.VisitEnumDecl(enumeration);
    }

    public override bool VisitEnumItemDecl(Enumeration.Item item)
    {
        if (IgnoreIfNotImGui(item, LogIgnoredImGuiEnumerationItem))
        {
            throw new InvalidOperationException(); // never happens
        }

        return base.VisitEnumItemDecl(item);
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
}