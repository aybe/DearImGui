using System.Reflection;
using CppSharp.AST;
using CppSharp.Types;
using Type = CppSharp.AST.Type;

namespace imgui.cppsharp.generator;

internal abstract class TypeMapIm : TypeMap
{
    public override Type CSharpSignatureType(TypePrinterContext ctx)
    {
        var attribute = GetType().GetCustomAttribute<TypeMapAttribute>() ?? throw new InvalidOperationException();
        var description = $"{Constants.Namespace}.{attribute.Type}";
        var customType = new CustomType(description);
        return customType;
    }
}