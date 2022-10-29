using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
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

        driver.AddGeneratorOutputPass(new CleanupTypes());

        if (Enhanced is true)
        {
        }

        driver.Generator.OnUnitGenerated += OnUnitGenerated;
    }

    public void Preprocess(Driver driver, ASTContext ctx)
    {
        Experimental.RemoveEnumerations(ctx);

        Experimental.FlattenNamespace(ctx);

        Experimental.RemovePasses(driver);

        ctx.SetClassAsValueType("ImVec2");
        ctx.SetClassAsValueType("ImVec4");

        ctx.IgnoreFunctionWithName("IM_DELETE");

        if (Enhanced is true)
        {
            return;

            // does nothing
            // Experimental.RemoveIndirection(ctx, "const ImVec2");
            // Experimental.RemoveIndirection(ctx, "const ImVec4");
        }
    }

    public void Postprocess(Driver driver, ASTContext ctx)
    {
        Experimental.IgnoreProperty(ctx, "ImVec2", "Item"); // BUG indexer setter
        Experimental.IgnoreProperty(ctx, "ImGuiStyle", "Colors"); // BUG indexer getter and setter
        Experimental.IgnoreProperty(ctx, "ImGuiIO", "MouseClickedPos"); // BUG indexer getter and setter
        Experimental.IgnoreProperty(ctx, "ImFontAtlas", "TexUvLines"); // BUG indexer getter and setter
        
        Experimental.IgnoreProperty(ctx, "ImVector", "Item"); // BUG indexer getter and setter
        Experimental.IgnoreProperty(ctx, "ImVector", "Data"); // BUG indexer getter and setter
        
        Experimental.IgnoreMethod(ctx, "ImFontAtlas", "GetMouseCursorTexData"); // BUG struct is not nullable

        if (Enhanced is true)
        {
            return;
            Experimental.IgnoreMembers(ctx);
        }
    }

    #endregion

    private static void OnUnitGenerated(GeneratorOutput output)
    {
        foreach (var generator in output.Outputs)
        {
            Experimental.UpdateHeader(generator);
        }
    }
}