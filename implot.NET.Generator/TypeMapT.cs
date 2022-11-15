﻿using CppSharp.Generators.CSharp;
using CppSharp.Types;
using im.NET.Generator.TypeMaps;
using JetBrains.Annotations;

namespace implot.NET.Generator;

[TypeMap("T")]
[UsedImplicitly]
internal sealed class TypeMapT : TypeMapBase
{
    public override void CSharpMarshalToNative(CSharpMarshalContext ctx)
    {
        ctx.Return.Write($"new IntPtr(Unsafe.AsPointer(ref {ctx.Parameter.Name}))");
        base.CSharpMarshalToNative(ctx);
    }
}