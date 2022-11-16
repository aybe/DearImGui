namespace im.NET.Generator;

public sealed class TypeNameComparer : Comparer<Type>
{
    public static TypeNameComparer Instance { get; } = new();

    public override int Compare(Type? x, Type? y)
    {
        return string.Compare(x?.Name, y?.Name, StringComparison.Ordinal);
    }
}