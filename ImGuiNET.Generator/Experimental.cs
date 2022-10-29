using System.Runtime.CompilerServices;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Passes;

namespace ImGuiNET.Generator;

internal static class Experimental
{
    public static void FlattenNamespace(ASTContext ctx)
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

    public static void IgnoreMembers(ASTContext ctx)
    {
        // TODO if using type map, these shall not be ignored else a bunch of errors will appear

        // ctx.IgnoreClassWithName("ImVec2");
        // ctx.IgnoreClassWithName("ImVec4");


        // removing ImVector introduces way more errors than keeping it
        // ctx.IgnoreClassWithName("ImVector");


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

    public static void RemoveEnumerations(ASTContext ctx)
    {
        ctx.FindCompleteEnum("ImGuiModFlags_").ExplicitlyIgnore();
        
        ctx.FindCompleteEnum("ImGuiNavInput_").ExplicitlyIgnore();

        foreach (var enumeration in ctx.TranslationUnits.SelectMany(s => s.Declarations).OfType<Enumeration>())
        {
            if (enumeration.Name.EndsWith("Private_", StringComparison.Ordinal))
            {
                enumeration.ExplicitlyIgnore();
                continue;
            }

            foreach (var item in enumeration.Items)
            {
                if (item.Name.EndsWith("_BEGIN", StringComparison.Ordinal))
                {
                    item.ExplicitlyIgnore();
                }

                if (item.Name.EndsWith("_END", StringComparison.Ordinal))
                {
                    item.ExplicitlyIgnore();
                }

                if (item.Name.EndsWith("_COUNT", StringComparison.Ordinal))
                {
                    item.ExplicitlyIgnore();
                }

                if (item.Name.EndsWith("_SIZE", StringComparison.Ordinal))
                {
                    item.ExplicitlyIgnore();
                }

                if (item.Name.EndsWith("_OFFSET", StringComparison.Ordinal))
                {
                    item.ExplicitlyIgnore();
                }
            }
        }
    }

    public static void RemovePasses(Driver driver, [CallerMemberName] string memberName = null!)
    {
        // WARNING
        // while this restores bunch of ignored methods, it changes tons of cctor to ctor too

        RemovePass<CheckIgnoredDeclsPass>(driver, memberName);
    }

    private static void RemovePass<T>(Driver driver, string memberName) where T : TranslationUnitPass
    {
        var count = driver.Context.TranslationUnitPasses.Passes.RemoveAll(s => s is T);

        Console.WriteLine($"### Removed {count} {typeof(T)} in {memberName}");
    }

    public static void RemoveIndirection(ASTContext ctx, string value)
    {
        var parameters =
            from unit in ctx.TranslationUnits
            from func in unit.Functions
            from para in func.Parameters
            where para.DebugText.Contains(value)
            select para;

        foreach (var parameter in parameters)
        {
            parameter.IsIndirect = false;
        }
    }

    public static void SetValueTypes(ASTContext ctx)
    {
        ctx.SetClassAsValueType("ImVec2");
        ctx.SetClassAsValueType("ImVec4");
    }

    public static void UpdateHeader(CodeGenerator generator)
    {
        var header = generator.FindBlock(BlockKind.Header);

        header.Text.WriteLine(
            "#pragma warning disable CS0109 // The member 'member' does not hide an inherited member. The new keyword is not required");

        var usings = generator.FindBlock(BlockKind.Usings);

        usings.Text.WriteLine(
            "using System.Runtime.CompilerServices;");
    }
}