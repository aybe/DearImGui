using CppSharp.AST;
using CppSharp.Generators.CSharp;
using Type = System.Type;

namespace imgui.NET.Generator.TypeMaps;

internal abstract class TypeMapImVec : TypeMapBase
{
    protected abstract Type TargetType { get; }

    public sealed override bool DoesMarshalling => base.DoesMarshalling;

    public sealed override bool IsIgnored => base.IsIgnored;

    public sealed override bool IsValueType => true;

    public sealed override CppSharp.AST.Type CSharpSignatureType(TypePrinterContext ctx)
    {
        return new CILType(TargetType);
    }

    public sealed override void CSharpMarshalToManaged(CSharpMarshalContext ctx)
    {
        if (ctx.Function == null)
        {
            if (ctx.ReturnVarName == null)
            {
                // NOP
            }
            else
            {
                ctx.Return.Write(ctx.ReturnVarName);
            }
        }
        else
        {
            if (ctx.ReturnVarName == null)
            {
                // NOP
            }
            else
            {
                if (ctx.ReturnType.Type is PointerType)
                {
                    ctx.Return.Write($"Unsafe.Read<global::{TargetType.FullName}>({ctx.ReturnVarName}.ToPointer())");
                }
                else
                {
                    ctx.Return.Write(ctx.ReturnVarName);
                }
            }
        }

        base.CSharpMarshalToManaged(ctx);
    }

    public sealed override void CSharpMarshalToNative(CSharpMarshalContext ctx)
    {
        if (ctx.Function == null)
        {
            if (ctx.ReturnVarName == null)
            {
                // NOP
            }
            else
            {
                ctx.Return.Write(ctx.Parameter.Name);
            }
        }
        else
        {
            if (ctx.ReturnVarName == null)
            {
                if (ctx.Parameter.IsConst || ctx.Parameter.HasDefaultValue is false)
                {
                    ctx.Return.Write($"new IntPtr(Unsafe.AsPointer(ref {ctx.Parameter.Name}))");
                }
                else
                {
                    ctx.Return.Write($"{ctx.Parameter.Name}");
                }
            }
            else
            {
                ctx.Return.Write(ctx.ReturnVarName);
            }
        }

        base.CSharpMarshalToNative(ctx);
    }
}