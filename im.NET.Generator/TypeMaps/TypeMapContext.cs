using CppSharp.AST;
using CppSharp.Generators.CSharp;
using CppSharp.Types;
using JetBrains.Annotations;
using Type = CppSharp.AST.Type;

// ReSharper disable RedundantIfElseBlock

namespace im.NET.Generator.TypeMaps;

[UsedImplicitly]
[TypeMap("ImGuiContext")]
internal sealed class TypeMapContext : TypeMapBase
{
    public override bool IsValueType => true;

    public override Type CSharpSignatureType(TypePrinterContext ctx)
    {
        return new CILType(typeof(IntPtr));
    }

    public override void CSharpMarshalToManaged(CSharpMarshalContext ctx)
    {
        if (ctx.Function is null)
        {
            if (ctx.ReturnVarName is null)
            {
                // NOP
            }
            else
            {
                // NOP
            }
        }
        else
        {
            if (ctx.ReturnVarName is null)
            {
                // NOP
            }
            else
            {
                ctx.Return.Write(ctx.ReturnVarName);
            }
        }

        base.CSharpMarshalToManaged(ctx);
    }

    public override void CSharpMarshalToNative(CSharpMarshalContext ctx)
    {
        if (ctx.Function is null)
        {
            if (ctx.ReturnVarName is null)
            {
                // NOP
            }
            else
            {
                // NOP
            }
        }
        else
        {
            if (ctx.ReturnVarName is null)
            {
                ctx.Return.Write(ctx.Parameter.Name);
            }
            else
            {
            }
        }

        base.CSharpMarshalToNative(ctx);
    }
}