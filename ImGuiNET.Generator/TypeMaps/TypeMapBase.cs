using System.Diagnostics;
using System.Runtime.CompilerServices;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;

namespace ImGuiNET.Generator.TypeMaps;

internal class TypeMapBase : TypeMap
{
    [Conditional("DEBUG_TYPE_MAP")]
    private void WriteDebugInformation(MarshalContext ctx, [CallerMemberName] string memberName = null!)
    {
        var text = $"{nameof(memberName)}: {GetType().Name}.{memberName}, " +
                   $"{nameof(ctx.Function)}: {ctx.Function != null}, " +
                   $"{nameof(ctx.ReturnVarName)}: {ctx.ReturnVarName != null}";

        var line = Environment.NewLine;

        ctx.Return.Write($"{line}/* {text} */{line}");
    }

    public override void CSharpMarshalToManaged(CSharpMarshalContext ctx)
    {
        WriteDebugInformation(ctx);
    }

    public override void CSharpMarshalToNative(CSharpMarshalContext ctx)
    {
        WriteDebugInformation(ctx);
    }
}