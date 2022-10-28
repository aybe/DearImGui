using System.Numerics;
using CppSharp.AST;
using CppSharp.Generators.CSharp;
using CppSharp.Types;
using JetBrains.Annotations;
using Type = CppSharp.AST.Type;

namespace ImGuiNET.Generator.TypeMaps;

[UsedImplicitly]
//[TypeMap("ImVec2", GeneratorKind.CSharp)] // TODO not working completely
internal sealed class MyTypeMapImVec2 : TypeMap
{
    public override Type CSharpSignatureType(TypePrinterContext ctx)
    {
        return new CILType(typeof(Vector2));
    }

    public override void CSharpMarshalToNative(CSharpMarshalContext ctx)
    {
        if (ctx.Function == null)
        {
            ctx.Return.Write(ctx.Parameter.Name);
        }
        else
        {
            ctx.Return.Write($"new IntPtr(Unsafe.AsPointer(ref {ctx.Parameter.Name}))");
        }
    }

    public override void CSharpMarshalToManaged(CSharpMarshalContext ctx)
    {
        if (ctx.ReturnVarName == "___ret")
        {
            ctx.Return.Write("new global::System.Numerics.Vector2(___ret.x, ___ret.y)");
        }
        else
        {
            ctx.Return.Write(ctx.ReturnVarName);
        }
    }
}