using System.Text.RegularExpressions;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;
using JetBrains.Annotations;
using Type = CppSharp.AST.Type;

namespace imgui.NET.Generator.TypeMaps;

[UsedImplicitly]
[TypeMap("ImVector", GeneratorKind.CSharp)]
internal sealed class TypeMapImVector : TypeMapBase
{
    public override Type CSharpSignatureType(TypePrinterContext ctx)
    {
        if ((ctx.Kind, ctx.MarshalKind) is (TypePrinterContextKind.Native, MarshalKind.NativeField))
            return new CustomType("ImVector.__Internal"); // auto-generated

        var args = ((TemplateSpecializationType)ctx.Type).Arguments[0].Type.Type;
        var type = new CustomType($"ImVector<{args}>");

        return type;
    }

    public override void CSharpMarshalToManaged(CSharpMarshalContext ctx)
    {
        if (ctx.Function == null)
        {
            if (ctx.ReturnVarName == null)
            {
                // NOP
            }
            else
            {
                var args = ((TemplateSpecializationType)ctx.ReturnType.Type).Arguments[0];
                var type = $"ImVector<{args}>";
                var data = Regex.Replace(ctx.ReturnVarName, @"^new __IntPtr\(&(.*)\)$", @"$1");
                var text = $"new {type}(Unsafe.As<ImVector.__Internal, {type}.__Internal>(ref {data}))";
                ctx.Return.Write(text);
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
                    ctx.Return.Write($"Unsafe.Read<ImVector>({ctx.ReturnVarName}.ToPointer())");
                }
                else
                {
                    ctx.Return.Write(ctx.ReturnVarName);
                }
            }
        }

        base.CSharpMarshalToManaged(ctx);
    }

    public override void CSharpMarshalToNative(CSharpMarshalContext ctx)
    {
        if (ctx.Function == null)
        {
            if (ctx.ReturnVarName == null)
            {
                // NOP
            }
            else
            {
                ctx.Return.Write(ctx.ReturnVarName); // invalid but gets pruned by SetPropertyAsReadOnly
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