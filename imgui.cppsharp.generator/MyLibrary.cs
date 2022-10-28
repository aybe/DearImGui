using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;

// ReSharper disable IdentifierTypo
// ReSharper disable RedundantIfElseBlock
// ReSharper disable CommentTypo
// ReSharper disable InvertIf
// ReSharper disable StringLiteralTypo

namespace imgui.cppsharp.generator;

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
            : @"..\..\..\..\imgui.cppsharp";

        options.GeneratorKind = GeneratorKind.CSharp;
        options.GenerateFinalizers = true;
        options.GenerateDebugOutput = true;
        options.MarshalCharAsManagedChar = true;
        options.Verbose = true;

        var module = options.AddModule("imgui");

        module.OutputNamespace = "imgui.NET";
        module.IncludeDirs.Add(@"..\..\..\..\imgui");
        module.Headers.Add("imconfig.h");
        module.Headers.Add("imgui.h");
    }

    public void SetupPasses(Driver driver)
    {
        driver.AddTranslationUnitPass(new EnumRenamePass());

        if (Enhanced is true)
        {
        }

        driver.Generator.OnUnitGenerated += OnUnitGenerated;
    }

    public void Preprocess(Driver driver, ASTContext ctx)
    {
        Experimental.RemoveEnumerations(ctx);

        if (Enhanced is true)
        {
            return;
            Experimental.RemovePasses(driver);

            // does nothing
            // Experimental.RemoveIndirection(ctx, "const ImVec2");
            // Experimental.RemoveIndirection(ctx, "const ImVec4");

            Experimental.SetValueTypes(ctx);

            Experimental.FlattenNamespace(ctx);
        }
    }

    public void Postprocess(Driver driver, ASTContext ctx)
    {
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