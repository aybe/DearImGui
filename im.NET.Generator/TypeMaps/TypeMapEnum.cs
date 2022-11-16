using System.Reflection;
using CppSharp.AST;
using CppSharp.Types;
using Type = CppSharp.AST.Type;

namespace im.NET.Generator.TypeMaps;

internal abstract class TypeMapEnum : TypeMap
{
    public override Type CSharpSignatureType(TypePrinterContext ctx)
    {
        var type = GetType();
        
        var attribute1 = type.GetCustomAttribute<TypeMapAttribute>() ?? throw new InvalidOperationException();
        var attribute2 = type.GetCustomAttribute<TypeMapEnumNamespaceAttribute>() ?? throw new InvalidOperationException();

        var customType = new CustomType($"global::{attribute2.Namespace}.{attribute1.Type}");

        return customType;
    }
}