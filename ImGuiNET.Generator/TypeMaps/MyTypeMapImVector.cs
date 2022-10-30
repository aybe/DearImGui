using System.Text.RegularExpressions;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;
using JetBrains.Annotations;
using Type = CppSharp.AST.Type;

namespace ImGuiNET.Generator.TypeMaps;

[UsedImplicitly]
[TypeMap("ImVector", GeneratorKind.CSharp)]
internal sealed class MyTypeMapImVector : TypeMapBase
{
    public override Type CSharpSignatureType(TypePrinterContext ctx)
    {
        if ((ctx.Kind, ctx.MarshalKind) is (TypePrinterContextKind.Native, MarshalKind.NativeField))
            return new CustomType("__IntPtr");

        var args = ((TemplateSpecializationType)ctx.Type).Arguments[0].Type.Type;
        var type = new CustomType($"global::ImGuiNET.ImVector<{args}>");

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
                var type = $"global::ImGuiNET.ImVector<{args}>";
                var data = Regex.Replace(ctx.ReturnVarName, @"^new __IntPtr\(&(.*)\)$", @"$1");
                var text = $"new {type}(Unsafe.Read<{type}.__Internal>({data}.ToPointer()))";
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
                    ctx.Return.Write($"Unsafe.Read<global::ImVector>({ctx.ReturnVarName}.ToPointer())");
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