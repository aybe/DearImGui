using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Passes;
using ImGuiNET.Generator.Passes;

// ReSharper disable IdentifierTypo
// ReSharper disable RedundantIfElseBlock
// ReSharper disable CommentTypo
// ReSharper disable InvertIf
// ReSharper disable StringLiteralTypo

namespace ImGuiNET.Generator;

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
            : @"..\..\..\..\ImGuiNET";

        options.GeneratorKind = GeneratorKind.CSharp;
        options.GenerateFinalizers = true;
        options.GenerateDebugOutput = true;
        options.MarshalCharAsManagedChar = true;
        options.Verbose = true;

        var module = options.AddModule("imgui");

        module.OutputNamespace = Constants.Namespace;
        module.IncludeDirs.Add(@"..\..\..\..\imgui");
        module.Headers.Add("imconfig.h");
        module.Headers.Add("imgui.h");
    }

    public void SetupPasses(Driver driver)
    {
        driver.AddTranslationUnitPass(new CleanupEnumerations());

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

        ctx.SetClassAsValueType("ImVec2");
        ctx.SetClassAsValueType("ImVec4");

        ctx.IgnoreFunctionWithName("IM_DELETE");

        if (Enhanced is true)
        {
            ctx.IgnoreClassWithName("ImColor");
            ctx.IgnoreClassWithName("ImVec2");
            ctx.IgnoreClassWithName("ImVec4");
        }
    }

    public void Postprocess(Driver driver, ASTContext ctx)
    {
        // stuff that is written manually
        Experimental.IgnoreProperty(ctx, "ImGuiStyle", "Colors");
        Experimental.IgnoreProperty(ctx, "ImGuiIO", "MouseClickedPos");
        Experimental.IgnoreProperty(ctx, "ImFontAtlas", "TexUvLines");

        // stuff that is yet to be done
        Experimental.IgnoreProperty(ctx, "ImVector", "Item"); // BUG indexer getter and setter
        Experimental.IgnoreProperty(ctx, "ImVector", "Data"); // BUG indexer getter and setter

        PostprocessDelegates(ctx);

        if (Enhanced is true)
        {
            var unit = GetImGuiTranslationUnit(ctx);

            var @class = unit.FindClass("ImVectorExtensions");

            @class.ExplicitlyIgnore();
        }
    }

    #endregion

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