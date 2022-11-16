namespace im.NET.Generator.TypeMaps;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class TypeMapEnumNamespaceAttribute : Attribute
{
    public TypeMapEnumNamespaceAttribute(string @namespace)
    {
        Namespace = @namespace;
    }

    public string Namespace { get; }
}