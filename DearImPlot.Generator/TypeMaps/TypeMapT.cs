using CppSharp.Generators.CSharp;
using CppSharp.Types;
using DearGenerator.TypeMaps;
using JetBrains.Annotations;

namespace DearImPlot.Generator.TypeMaps;

[TypeMap("T")]
[UsedImplicitly]
internal sealed class TypeMapT : TypeMapBase
{
    public override void CSharpMarshalToNative(CSharpMarshalContext ctx)
    {
        // fix the stuff for numerous methods that have 'const T* values' parameters
        ctx.Return.Write($"new IntPtr(Unsafe.AsPointer(ref {ctx.Parameter.Name}))");
        base.CSharpMarshalToNative(ctx);
    }
}