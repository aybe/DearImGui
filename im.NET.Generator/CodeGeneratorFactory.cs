using Microsoft.CodeAnalysis;

namespace im.NET.Generator;

public delegate CodeGenerator CodeGeneratorFactory(Platform platform, string directory);