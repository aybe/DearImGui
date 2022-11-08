using System.Diagnostics;
using System.Runtime.CompilerServices;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;

namespace imgui.NET.Generator.TypeMaps;

internal class TypeMapBase : TypeMap
{
    [Conditional("DEBUG_TYPE_MAP")]
    private void WriteDebugInformation(MarshalContext ctx, [CallerMemberName] string memberName = null!)
    {
        var text = $"{nameof(memberName)}: {GetType().Name}.{memberName}, " +
                   $"{nameof(ctx.Function)}: {ctx.Function != null}, " +
                   $"{nameof(ctx.ReturnVarName)}: {ctx.ReturnVarName != null}";

        var line = Environment.NewLine;

        var tabs = string.Empty.PadLeft((int)ctx.Return.CurrentIndentation * 4);

        var data = $"{line}{tabs}/* {text} */{line}{tabs}";

        ctx.Return.Write(data);
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