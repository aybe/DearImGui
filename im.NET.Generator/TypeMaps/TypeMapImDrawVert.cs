using CppSharp.AST;
using CppSharp.Types;
using im.NET.Generator;
using JetBrains.Annotations;
using Type = CppSharp.AST.Type;

// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

namespace imgui.NET.Generator.TypeMaps;

[TypeMap("ImDrawVert")]
[UsedImplicitly]
internal sealed class TypeMapImDrawVert : TypeMapBase
{
    public override Type CSharpSignatureType(TypePrinterContext ctx)
    {
        return new CustomType($"global::{Constants.ImGuiNamespace}.ImDrawVert");
    }
}