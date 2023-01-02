using Microsoft.CodeAnalysis;

namespace im.NET.Generator;

public delegate Generator GeneratorFactory(Platform platform, string directory);