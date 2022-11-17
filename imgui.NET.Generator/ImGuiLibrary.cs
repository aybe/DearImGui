﻿using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Passes;
using im.NET.Generator;
using im.NET.Generator.Logging;
using im.NET.Generator.Passes;
using imgui.NET.Generator.Passes;

// ReSharper disable IdentifierTypo
// ReSharper disable RedundantIfElseBlock
// ReSharper disable CommentTypo
// ReSharper disable InvertIf
// ReSharper disable StringLiteralTypo

namespace imgui.NET.Generator;

internal sealed class ImGuiLibrary : LibraryBase
{
    public bool? Enhanced { get; init; }

    public override void Setup(Driver driver)
    {
        base.Setup(driver);

        var module = driver.Options.AddModule("imgui");

        module.OutputNamespace = Constants.ImGuiNamespace;
        module.IncludeDirs.Add(@"..\..\..\..\imgui\imgui");
        module.Defines.Add("IMGUI_DISABLE_OBSOLETE_FUNCTIONS");
        module.Defines.Add("IMGUI_DISABLE_OBSOLETE_KEYIO");
        module.Headers.Add("imgui.h");
    }

    public override void SetupPasses(Driver driver)
    {
        driver.AddTranslationUnitPass(new ImEnumPass());
        driver.AddTranslationUnitPass(new ProduceSummary());

        if (Enhanced is true)
        {
        }

        driver.Generator.OnUnitGenerated += UnitGenerated;
    }

    public override void Preprocess(Driver driver, ASTContext ctx)
    {
        PreprocessPasses(driver);
        PreprocessEnumerations(ctx);
        PreprocessNamespace(ctx);
        PreprocessValueTypes(ctx);
        PreprocessIgnores(ctx);

        if (Enhanced is true)
        {
        }
    }

    public override void Postprocess(Driver driver, ASTContext ctx)
    {
        PostprocessIgnores(ctx);
        PostprocessDelegates(ctx);
        PostprocessEnumerations(ctx);
        PostprocessProperties(ctx);

        if (Enhanced is true)
        {
        }
    }

    #region Preprocess

    private static void PreprocessPasses(Driver driver)
    {
        // actually, we do want these, else we'll get pretty much nothing generated
        RemovePass<CheckIgnoredDeclsPass>(driver);

        // this is useless in our case, it also throws when adding our own comments
        RemovePass<CleanCommentsPass>(driver);
    }

    private static void PreprocessEnumerations(ASTContext ctx)
    {
        // hide some enumerations that aren't useful in our case

        ctx.FindCompleteEnum("ImGuiModFlags_").ExplicitlyIgnore();
        ctx.FindCompleteEnum("ImGuiNavInput_").ExplicitlyIgnore();
    }

    private static void PreprocessNamespace(ASTContext ctx)
    {
        // consolidate all of that stuff onto a unique namespace

        var unit = GetImGuiTranslationUnit(ctx);

        var ns = unit.Declarations.OfType<Namespace>().Single();

        var declarations = ns.Declarations.ToArray();

        ns.Declarations.Clear();

        unit.Declarations.AddRange(declarations);
    }

    private static void PreprocessValueTypes(ASTContext ctx)
    {
        // though ignored and manually implemented, we must set these as value types
        ctx.SetClassAsValueType("ImDrawVert");
        ctx.SetClassAsValueType("ImVec2");
        ctx.SetClassAsValueType("ImVec4");
    }

    private static void PreprocessIgnores(ASTContext ctx)
    {
        Ignore(ctx, "ImColor",    null, IgnoreType.Class); // unused
        Ignore(ctx, "ImDrawVert", null, IgnoreType.Class); // manual
        Ignore(ctx, "ImVec2",     null, IgnoreType.Class); // manual
        Ignore(ctx, "ImVec4",     null, IgnoreType.Class); // manual
        Ignore(ctx, "ImVector",   null, IgnoreType.Class); // manual

        Ignore(ctx, "ImDrawCmd",   "GetTexID",       IgnoreType.Method); // manual
        Ignore(ctx, "ImDrawList",  "GetClipRectMax", IgnoreType.Method); // manual
        Ignore(ctx, "ImDrawList",  "GetClipRectMin", IgnoreType.Method); // manual
        Ignore(ctx, "ImFont",      "GetDebugName",   IgnoreType.Method); // manual
        Ignore(ctx, "ImFont",      "IsLoaded",       IgnoreType.Method); // manual
        Ignore(ctx, "ImFontAtlas", "SetTexID",       IgnoreType.Method); // manual

        Ignore(ctx, null, "IM_DELETE", IgnoreType.Function); // unused
    }

    #endregion

    #region Postprocess

    private static void PostprocessIgnores(ASTContext ctx)
    {
        Ignore(ctx, "ImDrawData",            "CmdLists",        IgnoreType.Property); // manual
        Ignore(ctx, "ImDrawList",            "ClipRectStack",   IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "CmdHeader",       IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "FringeScale",     IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "IdxWritePtr",     IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "Path",            IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "Splitter",        IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "TextureIdStack",  IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "VtxCurrentIdx",   IgnoreType.Property); // intern
        Ignore(ctx, "ImDrawList",            "VtxWritePtr",     IgnoreType.Property); // intern
        Ignore(ctx, "ImFontAtlas",           "IsBuilt",         IgnoreType.Property); // manual
        Ignore(ctx, "ImFontAtlas",           "TexUvLines",      IgnoreType.Property); // manual
        Ignore(ctx, "ImFontAtlasCustomRect", "IsPacked",        IgnoreType.Property); // manual
        Ignore(ctx, "ImFontConfig",          "Name",            IgnoreType.Property); // manual
        Ignore(ctx, "ImGuiIO",               "MouseClickedPos", IgnoreType.Property); // manual
        Ignore(ctx, "ImGuiStyle",            "Colors",          IgnoreType.Property); // manual

        Ignore(ctx, "ImVectorExtensions", null, IgnoreType.Class); // unused
    }

    private static void PostprocessDelegates(ASTContext ctx)
    {
        // rename delegates to more appropriate names

        var tu = GetImGuiTranslationUnit(ctx);

        var ns = tu.FindNamespace("Delegates");

        ns.FindTypedef("Func___IntPtr___IntPtr")
            .Name = "ImGetClipboardTextHandler";

        ns.FindTypedef("Action___IntPtr_string8")
            .Name = "ImSetClipboardTextHandler";

        ns.FindTypedef("Action___IntPtr___IntPtr")
            .Name = "ImSetPlatformImeDataHandler";

        ns.FindTypedef("Func_bool___IntPtr_int_sbytePtrPtr")
            .Name = "ImItemsGetterHandler";

        ns.FindTypedef("Func_float___IntPtr_int")
            .Name = "ImValuesGetterHandler";

        // merge these delegates with upper namespace

        foreach (var declaration in ns.Declarations)
        {
            declaration.Namespace = tu;

            using (new ConsoleColorScope(null, ConsoleColor.Yellow))
                Console.WriteLine($"Set declaration {declaration} namespace to {tu}");
        }

        tu.Declarations.AddRange(ns.Declarations);

        ns.Declarations.Clear();
    }

    private static void PostprocessEnumerations(ASTContext ctx)
    {
        SetEnumerationsFlags(GetImGuiTranslationUnit(ctx));
    }

    private static void PostprocessProperties(ASTContext ctx)
    {
        // vector properties are not meant to be assignable, make them read-only

        var unit = GetImGuiTranslationUnit(ctx);

        foreach (var c in unit.Classes)
        {
            foreach (var p in c.Properties)
            {
                if (p.QualifiedType.Type is not TemplateSpecializationType type)
                    continue;

                if (type.Template.Name is not "ImVector")
                    continue;

                ctx.SetPropertyAsReadOnly(c.Name, p.Name);

                using (new ConsoleColorScope(null, ConsoleColor.Yellow))
                    Console.WriteLine($"Set ImVector<T> property as read-only: {c.Name}.{p.Name}");
            }
        }
    }

    private static void UnitGenerated(GeneratorOutput output)
    {
        foreach (var generator in output.Outputs)
        {
            UpdateGeneratedContent(generator);
        }
    }

    private static void UpdateGeneratedContent(CodeGenerator generator)
    {
        var header = generator.FindBlock(BlockKind.Header);

        header.Text.WriteLine(
            "#pragma warning disable CS0109 // The member 'member' does not hide an inherited member. The new keyword is not required");

        var usings = generator.FindBlock(BlockKind.Usings);

        usings.Text.WriteLine("using System.Collections.Concurrent;");
        usings.Text.WriteLine("using System.Numerics;");
        usings.Text.WriteLine("using System.Runtime.CompilerServices;");
        usings.Text.WriteLine("using System.Text;");

        var comments = generator.FindBlocks(BlockKind.BlockComment);

        foreach (var comment in comments)
        {
            comment.Text.StringBuilder.Replace("&lt;br/&gt;", "<br/>");
        }
    }

    #endregion
}