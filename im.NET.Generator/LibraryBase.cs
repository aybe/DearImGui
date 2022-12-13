using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Passes;
using im.NET.Generator.Logging;
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

namespace im.NET.Generator;

[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
public abstract class LibraryBase : ILibrary
{
    protected static TranslationUnit GetImGuiTranslationUnit(ASTContext ctx)
    {
        return GetTranslationUnit(ctx, "imgui.h");
    }

    protected static TranslationUnit GetImPlotTranslationUnit(ASTContext ctx)
    {
        return GetTranslationUnit(ctx, "implot.h");
    }

    private static TranslationUnit GetTranslationUnit(ASTContext ctx, string fileName)
    {
        return ctx.TranslationUnits.Single(s => s.FileName == fileName);
    }

    protected static void Ignore(ASTContext ctx, string? className, string? memberName, IgnoreType ignoreType)
    {
        switch (ignoreType)
        {
            case IgnoreType.Class:
                ctx.IgnoreClassWithName(className);
                return;
            case IgnoreType.Enum:
                ctx.FindCompleteEnum(className).ExplicitlyIgnore();
                return;
            case IgnoreType.Function:
                ctx.IgnoreFunctionWithName(memberName);
                return;
            case IgnoreType.Method:
                ctx.IgnoreClassMethodWithName(className, memberName);
                return;
            case IgnoreType.Property:
                ctx.FindCompleteClass(className).Properties.Single(s => s.Name == memberName).ExplicitlyIgnore();
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(ignoreType), ignoreType, null);
        }
    }

    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
    protected virtual void PreprocessPasses(Driver driver)
    {
        // actually, we do want these, else we'll get pretty much nothing generated

        RemovePass<CheckIgnoredDeclsPass>(driver);

        // this is useless in our case, it also throws when adding our own comments

        RemovePass<CleanCommentsPass>(driver);
    }

    protected virtual void PreprocessValueTypes(ASTContext ctx)
    {
        ctx.SetClassAsValueType("ImDrawCmd");
        ctx.SetClassAsValueType("ImDrawData");
        ctx.SetClassAsValueType("ImDrawList");
        ctx.SetClassAsValueType("ImDrawVert");
        ctx.SetClassAsValueType("ImVec2");
        ctx.SetClassAsValueType("ImVec4");
    }

    protected virtual void PreprocessIgnores(ASTContext ctx)
    {
        Ignore(ctx, "ImColor",    null, IgnoreType.Class); // unused
        Ignore(ctx, "ImDrawVert", null, IgnoreType.Class); // manual
        Ignore(ctx, "ImVec2",     null, IgnoreType.Class); // manual
        Ignore(ctx, "ImVec4",     null, IgnoreType.Class); // manual
        Ignore(ctx, "ImVector",   null, IgnoreType.Class); // manual

        Ignore(ctx, "ImGuiModFlags_", null, IgnoreType.Enum); // useless
        Ignore(ctx, "ImGuiNavInput_", null, IgnoreType.Enum); // useless

        Ignore(ctx, "ImDrawCmd",   "GetTexID",       IgnoreType.Method); // manual
        Ignore(ctx, "ImDrawList",  "GetClipRectMax", IgnoreType.Method); // manual
        Ignore(ctx, "ImDrawList",  "GetClipRectMin", IgnoreType.Method); // manual
        Ignore(ctx, "ImFont",      "GetDebugName",   IgnoreType.Method); // manual
        Ignore(ctx, "ImFont",      "IsLoaded",       IgnoreType.Method); // manual
        Ignore(ctx, "ImFontAtlas", "SetTexID",       IgnoreType.Method); // manual

        Ignore(ctx, null, "IM_DELETE", IgnoreType.Function); // unused
    }

    protected static void PushClassDeclarationsUpstream(TranslationUnit unit, string @class)
    {
        var ns = unit.Namespaces.Single(s => s.Name == @class);

        unit.Declarations.AddRange(ns.Declarations);

        ns.Declarations.Clear();
    }

    private static void RemovePass<T>(Driver driver, [CallerMemberName] string memberName = null!) where T : TranslationUnitPass
    {
        var count = driver.Context.TranslationUnitPasses.Passes.RemoveAll(s => s is T);

        using (new ConsoleColorScope(null, ConsoleColor.Yellow))
        {
            Console.WriteLine($"Removed {count} passes of type {typeof(T)} in {memberName}");
        }
    }

    protected static void SetEnumerationsFlags(TranslationUnit unit)
    {
        foreach (var enumeration in unit.Enums)
        {
            if (enumeration.Name.Contains("Flags") is false)
            {
                continue;
            }

            if (enumeration.IsFlags)
            {
                continue;
            }

            enumeration.SetFlags();

            using (new ConsoleColorScope(null, ConsoleColor.Yellow))
            {
                Console.WriteLine($"Set enumeration as flags: {enumeration.Name}");
            }
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    protected static void SetupImGui(Module module)
    {
        module.IncludeDirs.Add(@"..\..\..\..\imgui\imgui");
        module.Defines.Add("IMGUI_DISABLE_OBSOLETE_FUNCTIONS");
        module.Defines.Add("IMGUI_DISABLE_OBSOLETE_KEYIO");
        module.Headers.Add("imgui.h");
    }

    #region ILibrary Members

    public virtual void Setup(Driver driver)
    {
        var options = driver.Options;

        options.GeneratorKind = GeneratorKind.CSharp;
        options.GenerateFinalizers = true;
        options.GenerateDebugOutput = true;
        options.GenerateDefaultValuesForArguments = true;
        options.MarshalCharAsManagedChar = true;
        options.UseSpan = true;
        options.Verbose = true;
    }

    public abstract void SetupPasses(Driver driver);

    public virtual void Preprocess(Driver driver, ASTContext ctx)
    {
        PreprocessPasses(driver);
        PreprocessValueTypes(ctx);
        PreprocessIgnores(ctx);
    }

    public virtual void Postprocess(Driver driver, ASTContext ctx)
    {
        PostprocessIgnores(ctx);
        PostprocessEnumerations(ctx);
        PostprocessDeclarations(ctx);
        PostprocessDelegates(ctx);
        PostprocessProperties(ctx);
    }

    #endregion

    protected virtual void PostprocessIgnores(ASTContext ctx)
    {
    }

    protected virtual void PostprocessEnumerations(ASTContext ctx)
    {
    }

    protected virtual void PostprocessDeclarations(ASTContext ctx)
    {
    }

    protected virtual void PostprocessDelegates(ASTContext ctx)
    {
    }

    protected virtual void PostprocessProperties(ASTContext ctx)
    {
    }
}