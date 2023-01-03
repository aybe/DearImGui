using System.Runtime.CompilerServices;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;

namespace DearGenerator.TypeMaps;

public class TypeMapBase : TypeMap
{
    protected void WriteDebugInformation(MarshalContext ctx, string? message = null, [CallerMemberName] string? memberName = null)
    {
        // write some debug information as the numerous cases are confusing

        message ??= $"{nameof(ctx.Function)}: {ctx.Function != null}, {nameof(ctx.ReturnVarName)}: {ctx.ReturnVarName != null}";

        var text = $"{GetType().Name}.{memberName}: {message}";

        var line = Environment.NewLine;

        var tabs = string.Empty.PadLeft((int)ctx.Return.CurrentIndentation * 4);

        var data = $"{line}{tabs}/* DEBUG: {text} */{line}{tabs}";

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