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

        var name =
            type
                .GetCustomAttributes<TypeMapAttribute>(true)
                .FirstOrDefault(s => Context.ASTContext.FindEnum(s.Type).Any())?.Type
            ?? throw new InvalidOperationException("Couldn't find enumeration.");

        var ns =
            type
                .GetCustomAttribute<TypeMapEnumNamespaceAttribute>()
            ?? throw new InvalidOperationException();

        var tn = $"global::{ns.Namespace}.{name}";

        var ct = new CustomType(tn);

        return ct;
    }
}