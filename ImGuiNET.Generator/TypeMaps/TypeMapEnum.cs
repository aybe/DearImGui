using System.Reflection;
using CppSharp.AST;
using CppSharp.Types;
using Type = CppSharp.AST.Type;

namespace imgui.NET.Generator.TypeMaps;

internal abstract class TypeMapEnum : TypeMap
{
    public override Type CSharpSignatureType(TypePrinterContext ctx)
    {
        var attribute = GetType().GetCustomAttribute<TypeMapAttribute>() ?? throw new InvalidOperationException();
        var description = $"{Constants.Namespace}.{attribute.Type}";
        var customType = new CustomType(description);
        return customType;
    }
}