using System.Numerics;
using CppSharp.Generators;
using CppSharp.Types;
using JetBrains.Annotations;

namespace DearGenerator.TypeMaps;

[UsedImplicitly]
[TypeMap("ImVec2", GeneratorKind.CSharp)]
internal sealed class TypeMapImVec2 : TypeMapImVec
{
    protected override Type TargetType { get; } = typeof(Vector2);
}