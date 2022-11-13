using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CppSharp;
using im.NET.Generator;
using implot.NET.Generator;

if (Debugger.IsAttached) // cleanup garbage
{
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Clear();
}

var namespaces = new SortedSet<string>
    {
        "implot.NET",
        "System",
        "System.Collections.Concurrent",
        "System.Numerics",
        "System.Runtime.CompilerServices",
        "System.Runtime.InteropServices",
        "System.Security",
        "System.Text"
    }
    .ToImmutableSortedSet();

var classes = new SortedSet<KeyValuePair<string, string>>
    {
        new("implot", "ImPlot")
    }
    .ToImmutableSortedSet();

var aliases = new SortedSet<Type>(TypeComparer.Instance)
    {
        typeof(CallingConvention),
        typeof(IntPtr)
    }
    .ToImmutableSortedSet(TypeComparer.Instance);

var library = new ImPlotLibrary
{
    GeneratorType = GeneratorType.ImPlot,
    Namespaces = namespaces
};

ConsoleDriver.Run(library);

var path = Path.Combine(Environment.CurrentDirectory, "implot.cs");

var text = File.ReadAllText(path);

RenameClasses(ref text, classes);

ShortenQualifiers(ref text, namespaces);

RepairUsingAliases(ref text, aliases);

File.WriteAllText(path, text);

static void RepairUsingAliases(ref string input, ImmutableSortedSet<Type> aliases)
{
    foreach (var item in aliases)
    {
        input = input.Replace($"__{item.Name}", item.Name);
    }
}

static void RenameClasses(ref string input, ImmutableSortedSet<KeyValuePair<string, string>> classes)
{
    foreach (var item in classes)
    {
        input = input.Replace($"class {item.Key}", $"class {item.Value}");
    }
}

static void ShortenQualifiers(ref string input, ImmutableSortedSet<string> namespaces)
{
    foreach (var item in namespaces.Reverse())
    {
        input = input.Replace($"global::{item}.", string.Empty);
    }
}

internal sealed class TypeComparer : Comparer<Type>
{
    public static TypeComparer Instance { get; } = new();

    public override int Compare(Type? x, Type? y)
    {
        return string.Compare(x?.Name, y?.Name, StringComparison.Ordinal);
    }
}