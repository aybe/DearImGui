using System.Numerics;
using CppSharp.Generators;
using CppSharp.Types;
using JetBrains.Annotations;

namespace DearGenerator.TypeMaps;

[UsedImplicitly]
[TypeMap("ImVec4", GeneratorKind.CSharp)]
internal sealed class TypeMapImVec4 : TypeMapImVec
{
    protected override Type TargetType { get; } = typeof(Vector4);
}