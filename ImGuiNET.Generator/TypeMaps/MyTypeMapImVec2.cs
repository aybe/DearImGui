using System.Numerics;
using CppSharp.Generators;
using CppSharp.Types;
using JetBrains.Annotations;

namespace ImGuiNET.Generator.TypeMaps;

[UsedImplicitly]
[TypeMap("ImVec2", GeneratorKind.CSharp)]
internal sealed class MyTypeMapImVec2 : MyTypeMapImVec
{
    protected override Type TargetType { get; } = typeof(Vector2);
}