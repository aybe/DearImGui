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
        driver.Generator.OnUnitGenerated += OnUnitGenerated;
    }

    public void Preprocess(Driver driver, ASTContext ctx)
    {
        if (Enhanced is true)
        {
            Experimental.RemovePasses(driver);
        }

        SetValueTypes(ctx);

        FlattenNamespace(ctx);
    }

    public void Postprocess(Driver driver, ASTContext ctx)
    {
        IgnoreMembers(ctx);
    }

    #endregion

    private static void SetValueTypes(ASTContext ctx)
    {
        ctx.SetClassAsValueType("ImVec2");
        ctx.SetClassAsValueType("ImVec4");
    }

    private static void FlattenNamespace(ASTContext ctx)
    {
        // TODO rename imgui class to ImGui

        foreach (var unit in ctx.TranslationUnits)
        {
            if (unit.FileName != "imgui.h")
                continue;

            var ns = unit.Declarations.OfType<Namespace>().Single();

            var declarations = ns.Declarations.ToArray();

            ns.Declarations.Clear();

            unit.Declarations.AddRange(declarations);
        }
    }

    private static void IgnoreMembers(ASTContext ctx)
    {
        // TODO if using type map, these shall not be ignored else a bunch of errors will appear

        // ctx.IgnoreClassWithName("ImVec2");
        // ctx.IgnoreClassWithName("ImVec4");


        // TODO implement these members manually

        IgnoreMethod(ctx, "ImFontAtlas", "GetMouseCursorTexData"); // BUG struct is not nullable

        IgnoreProperty(ctx, "ImVec2", "Item"); // BUG indexer setter
        IgnoreProperty(ctx, "ImGuiStyle", "Colors"); // BUG indexer getter and setter
        IgnoreProperty(ctx, "ImGuiIO", "MouseClickedPos"); // BUG indexer getter and setter
        IgnoreProperty(ctx, "ImFontAtlas", "TexUvLines"); // BUG indexer getter and setter
    }

    private static void IgnoreMethod(ASTContext ctx, string className, string methodName)
    {
        var c = ctx.FindCompleteClass(className);
        var m = c.Methods.Single(s => s.Name == methodName);

        m.ExplicitlyIgnore();
    }

    private static void IgnoreProperty(ASTContext ctx, string className, string propertyName)
    {
        var c = ctx.FindCompleteClass(className);
        var p = c.Properties.Single(s => s.Name == propertyName);

        p.ExplicitlyIgnore();
    }

    private static void OnUnitGenerated(GeneratorOutput output)
    {
        foreach (var generator in output.Outputs)
        {
            UpdateHeader(generator);
        }
    }

    private static void UpdateHeader(CodeGenerator generator)
    {
        var header = generator.FindBlock(BlockKind.Header);

        header.Text.WriteLine(
            "#pragma warning disable CS0109 // The member 'member' does not hide an inherited member. The new keyword is not required");

        var usings = generator.FindBlock(BlockKind.Usings);

        usings.Text.WriteLine(
            "using System.Runtime.CompilerServices;");
    }
}