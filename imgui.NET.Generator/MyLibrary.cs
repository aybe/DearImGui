using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Passes;
using imgui.NET.Generator.Passes;

// ReSharper disable IdentifierTypo
// ReSharper disable RedundantIfElseBlock
// ReSharper disable CommentTypo
// ReSharper disable InvertIf
// ReSharper disable StringLiteralTypo

namespace imgui.NET.Generator;

internal sealed class MyLibrary : ILibrary
{
    public bool? Enhanced { get; init; }

    #region ILibrary Members

    public void Setup(Driver driver)
    {
        var options = driver.Options;

        options.OutputDir = Enhanced.HasValue
            ? Enhanced.Value
                ? "NEW"
                : "OLD"
            : @"..\..\..\..\imgui.NET";

        options.GeneratorKind = GeneratorKind.CSharp;
        options.GenerateFinalizers = true;
#if DEBUG
        options.GenerateDebugOutput = true;
#endif
        options.MarshalCharAsManagedChar = true;
        options.Verbose = true;

        var module = options.AddModule("imgui");

        module.OutputNamespace = Constants.Namespace;
        module.IncludeDirs.Add(@"..\..\..\..\imgui\imgui");
        module.Defines.Add("IMGUI_DISABLE_OBSOLETE_FUNCTIONS");
        module.Defines.Add("IMGUI_DISABLE_OBSOLETE_KEYIO");
        module.Headers.Add("imgui.h");
    }

    public void SetupPasses(Driver driver)
    {
        driver.AddTranslationUnitPass(new CleanupEnumerations());
        driver.AddTranslationUnitPass(new ProduceSummary());

        if (Enhanced is true)
        {
        }

        driver.Generator.OnUnitGenerated += OnUnitGenerated;
    }

    public void Preprocess(Driver driver, ASTContext ctx)
    {
        Experimental.RemoveEnumerations(ctx);

        Experimental.FlattenNamespace(ctx);

        Experimental.RemovePass<CheckIgnoredDeclsPass>(driver);

        Experimental.RemovePass<CleanCommentsPass>(driver); // useless, throws when adding our comments to functions

        {
            // whenever we ignore a type to implement it manually, we still have to set value type where appropriate

            ctx.SetClassAsValueType("ImDrawVert");
            ctx.IgnoreClassWithName("ImDrawVert");

            ctx.SetClassAsValueType("ImVec2");
            ctx.IgnoreClassWithName("ImVec2");

            ctx.SetClassAsValueType("ImVec4");
            ctx.IgnoreClassWithName("ImVec4");
        }

        ctx.IgnoreClassWithName("ImColor");
        ctx.IgnoreClassWithName("ImVector");

        ctx.IgnoreClassMethodWithName("ImDrawCmd", "GetTexID");
        ctx.IgnoreConversionToProperty("ImDrawList::GetClipRectMin"); // TODO
        ctx.IgnoreConversionToProperty("ImDrawList::GetClipRectMax"); // TODO

        ctx.IgnoreFunctionWithName("IM_DELETE");

        if (Enhanced is true)
        {
        }
    }

    public void Postprocess(Driver driver, ASTContext ctx)
    {
        Experimental.Ignore(ctx, "ImDrawData",         "CmdLists",        IgnoreType.Property); // manual
        Experimental.Ignore(ctx, "ImDrawList",         "ClipRectStack",   IgnoreType.Property); // intern
        Experimental.Ignore(ctx, "ImDrawList",         "CmdHeader",       IgnoreType.Property); // intern
        Experimental.Ignore(ctx, "ImDrawList",         "FringeScale",     IgnoreType.Property); // intern
        Experimental.Ignore(ctx, "ImDrawList",         "IdxWritePtr",     IgnoreType.Property); // intern
        Experimental.Ignore(ctx, "ImDrawList",         "Path",            IgnoreType.Property); // intern
        Experimental.Ignore(ctx, "ImDrawList",         "Splitter",        IgnoreType.Property); // intern
        Experimental.Ignore(ctx, "ImDrawList",         "TextureIdStack",  IgnoreType.Property); // intern
        Experimental.Ignore(ctx, "ImDrawList",         "VtxCurrentIdx",   IgnoreType.Property); // intern
        Experimental.Ignore(ctx, "ImDrawList",         "VtxWritePtr",     IgnoreType.Property); // intern
        Experimental.Ignore(ctx, "ImFontAtlas",        "IsBuilt",         IgnoreType.Property); // manual
        Experimental.Ignore(ctx, "ImFontAtlas",        "SetTexID",        IgnoreType.Method);   // manual
        Experimental.Ignore(ctx, "ImFontAtlas",        "TexUvLines",      IgnoreType.Property); // manual
        Experimental.Ignore(ctx, "ImGuiIO",            "MouseClickedPos", IgnoreType.Property); // manual
        Experimental.Ignore(ctx, "ImGuiStyle",         "Colors",          IgnoreType.Property); // manual
        Experimental.Ignore(ctx, "ImVectorExtensions", null,              IgnoreType.Class);    // unused

        PostprocessDelegates(ctx);
        PostprocessProperties(ctx);

        ctx.SetEnumAsFlags("ImDrawFlags");
        ctx.SetEnumAsFlags("ImGuiButtonFlags");
        ctx.SetEnumAsFlags("ImGuiColorEditFlags");
        ctx.SetEnumAsFlags("ImGuiComboFlags");
        ctx.SetEnumAsFlags("ImGuiDragDropFlags");
        ctx.SetEnumAsFlags("ImGuiFocusedFlags");
        ctx.SetEnumAsFlags("ImGuiHoveredFlags");
        ctx.SetEnumAsFlags("ImGuiPopupFlags");
        ctx.SetEnumAsFlags("ImGuiSliderFlags");
        ctx.SetEnumAsFlags("ImGuiTabBarFlags");
        ctx.SetEnumAsFlags("ImGuiTableColumnFlags");
        ctx.SetEnumAsFlags("ImGuiTableFlags");
        ctx.SetEnumAsFlags("ImGuiTableRowFlags");
        ctx.SetEnumAsFlags("ImGuiTreeNodeFlags");
        ctx.SetEnumAsFlags("ImGuiWindowFlags");

        if (Enhanced is true)
        {
        }
    }

    #endregion

    private static void PostprocessProperties(ASTContext ctx)
    {
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

                Console.WriteLine($"ImVector<T> property set as read-only: {c.Name}.{p.Name}");
            }
        }
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

        // move delegates to upper namespace

        foreach (var declaration in ns.Declarations)
        {
            declaration.Namespace = tu;
        }

        tu.Declarations.AddRange(ns.Declarations);

        ns.Declarations.Clear();
    }

    private static TranslationUnit GetImGuiTranslationUnit(ASTContext ctx)
    {
        return ctx.TranslationUnits.Single(s => s.FileName == "imgui.h");
    }

    private static void OnUnitGenerated(GeneratorOutput output)
    {
        foreach (var generator in output.Outputs)
        {
            Experimental.UpdateHeader(generator);
        }
    }
}