using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Passes;
using im.NET.Generator.Logging;

namespace im.NET.Generator;

public abstract class LibraryBase : ILibrary
{
    public abstract ImmutableSortedSet<string> Namespaces { get; init; }

    #region ILibrary Members

    public virtual void Setup(Driver driver)
    {
        var options = driver.Options;

        options.GeneratorKind = GeneratorKind.CSharp;
        options.GenerateFinalizers = true;
#if DEBUG
        options.GenerateDebugOutput = true;
#endif
        options.MarshalCharAsManagedChar = true;
        options.Verbose = true;
    }

    public abstract void SetupPasses(Driver driver);

    public abstract void Preprocess(Driver driver, ASTContext ctx);

    public abstract void Postprocess(Driver driver, ASTContext ctx);

    #endregion

    protected static TranslationUnit GetImGuiTranslationUnit(ASTContext ctx)
    {
        return ctx.TranslationUnits.Single(s => s.FileName == "imgui.h");
    }

    protected static void Ignore(ASTContext ctx, string? className, string? memberName, IgnoreType ignoreType)
    {
        switch (ignoreType)
        {
            case IgnoreType.Class:
                ctx.IgnoreClassWithName(className);
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

    protected void ProcessSources(CSharpSources sources)
    {
        // disable some warnings

        var header = sources.FindBlock(BlockKind.Header);

        header.Text.WriteLine(
            "#pragma warning disable CS0109 // The member 'member' does not hide an inherited member. The new keyword is not required");

        // add used usings

        var usings = sources.FindBlock(BlockKind.Usings);

        usings.Text.StringBuilder.Clear();

        foreach (var item in Namespaces)
        {
            usings.Text.WriteLine($"using {item};");
        }

        // fix escaped new lines in comments

        var comments = sources.FindBlocks(BlockKind.BlockComment);

        foreach (var comment in comments)
        {
            comment.Text.StringBuilder.Replace("&lt;br/&gt;", "<br/>");
        }
    }

    protected static void RemovePass<T>(Driver driver, [CallerMemberName] string memberName = null!) where T : TranslationUnitPass
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
                continue;

            if (enumeration.IsFlags)
                continue;

            enumeration.SetFlags();

            using (new ConsoleColorScope(null, ConsoleColor.Yellow))
            {
                Console.WriteLine($"Set enumeration as flags: {enumeration.Name}");
            }
        }
    }
}