using System.Runtime.CompilerServices;
using CppSharp;
using CppSharp.AST;
using CppSharp.Passes;

namespace imgui.cppsharp.generator;

internal static class Experimental
{
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
}