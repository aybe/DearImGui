using Microsoft.CodeAnalysis;

namespace im.NET.Generator;

public delegate CodeGenerator GeneratorFactory(Platform platform, string directory);