using CppSharp.AST;
using CppSharp.Generators.CSharp;
using CppSharp.Types;
using JetBrains.Annotations;
using Type = CppSharp.AST.Type;

// ReSharper disable RedundantIfElseBlock

namespace DearGenerator.TypeMaps;

[TypeMap("size_t")]
[UsedImplicitly]
internal sealed class TypeMapSizeT : TypeMapBase
{
    public override Type CSharpSignatureType(TypePrinterContext ctx)
    {
        return new CILType(typeof(nuint)); // will end up as UIntPtr
    }

    public override void CSharpMarshalToManaged(CSharpMarshalContext ctx)
    {
        throw new InvalidOperationException("This should never occur.");
    }

    public override void CSharpMarshalToNative(CSharpMarshalContext ctx)
    {
        if (ctx.Function == null)
        {
            if (ctx.ReturnVarName == null)
            {
                throw new InvalidOperationException("This should never occur.");
            }
            else
            {
                throw new InvalidOperationException("This should never occur.");
            }
        }
        else
        {
            if (ctx.ReturnVarName == null)
            {
                ctx.Return.Write(ctx.Parameter.Name);
            }
            else
            {
                throw new InvalidOperationException("This should never occur.");
            }
        }

        base.CSharpMarshalToNative(ctx);
    }
}