#define DEBUG_TYPE_MAP
using CppSharp.AST;
using CppSharp.Generators.CSharp;
using CppSharp.Types;
using Type = System.Type;

namespace ImGuiNET.Generator.TypeMaps;

internal abstract class MyTypeMapImVec : TypeMap
{
    protected abstract Type TargetType { get; }

    public sealed override bool DoesMarshalling => base.DoesMarshalling;

    public sealed override bool IsIgnored => base.IsIgnored;

    public sealed override bool IsValueType => true;

    public sealed override CppSharp.AST.Type CSharpSignatureType(TypePrinterContext ctx)
    {
        return new CILType(TargetType);
    }

    public sealed override void CSharpMarshalToNative(CSharpMarshalContext ctx)
    {
        if (ctx.Function == null)
        {
            if (ctx.ReturnVarName == null)
            {
#if DEBUG_TYPE_MAP
                ctx.Return.Write("/* CSharpMarshalToNative func null, return var name null */");
#endif
            }
            else
            {
                ctx.Return.Write(ctx.Parameter.Name);
#if DEBUG_TYPE_MAP
                ctx.Return.Write("/* CSharpMarshalToNative func null, return var name NOT null */");
#endif
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

#if DEBUG_TYPE_MAP
                ctx.Return.Write("/* CSharpMarshalToNative func NOT null, return var name null */");
#endif
            }
            else
            {
                ctx.Return.Write(ctx.ReturnVarName);
#if DEBUG_TYPE_MAP
                ctx.Return.Write("/* CSharpMarshalToNative func NOT null, return var name NOT null */");
#endif
            }
        }
    }

    public sealed override void CSharpMarshalToManaged(CSharpMarshalContext ctx)
    {
        if (ctx.Function == null)
        {
            if (ctx.ReturnVarName == null)
            {
#if DEBUG_TYPE_MAP
                ctx.Return.Write("/* CSharpMarshalToManaged func null, return var name null */");
#endif
            }
            else
            {
                ctx.Return.Write(ctx.ReturnVarName);
#if DEBUG_TYPE_MAP
                ctx.Return.Write("/* CSharpMarshalToManaged func null, return var name NOT null */");
#endif
            }
        }
        else
        {
            if (ctx.ReturnVarName == null)
            {
#if DEBUG_TYPE_MAP
                ctx.Return.Write("/* CSharpMarshalToManaged func NOT null, return var name null */");
#endif
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

#if DEBUG_TYPE_MAP
                ctx.Return.Write("/* CSharpMarshalToManaged func NOT null, return var name NOT null */");
#endif
            }
        }
    }
}