using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using CppSharp.AST;
using CppSharp.Generators.CSharp;
using Type = System.Type;

// ReSharper disable RedundantIfElseBlock

namespace im.NET.Generator.TypeMaps;

[SuppressMessage("ReSharper", "ConvertIfStatementToConditionalTernaryExpression")]
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
                var match = Regex.Match(ctx.ReturnVarName, @"(?<=^new\s__IntPtr\(&).*(?=\)$)");

                if (string.IsNullOrEmpty(match.Value))
                {
                    ctx.Return.Write(ctx.ReturnVarName); // imgui
                }
                else
                {
                    ctx.Return.Write(match.Value); // implot
                }
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
                var asIntPtr = $"new IntPtr(Unsafe.AsPointer(ref {ctx.Parameter.Name}))";

                if (ctx.Parameter.IsConst)
                {
                    if (ctx.Parameter.HasDefaultValue)
                    {
                        ctx.Return.Write(asIntPtr);
                    }
                    else
                    {
                        ctx.Return.Write(asIntPtr);
                    }
                }
                else
                {
                    if (ctx.Parameter.HasDefaultValue)
                    {
                        switch (ctx.Parameter.Type)
                        {
                            case TagType:
                                ctx.Return.Write(ctx.Parameter.Name); // imgui
                                break;
                            case PointerType:
                                ctx.Return.Write(asIntPtr); // implot
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        ctx.Return.Write(asIntPtr);
                    }
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