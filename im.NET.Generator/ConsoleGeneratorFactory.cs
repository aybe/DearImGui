using Microsoft.CodeAnalysis;

namespace im.NET.Generator;

public delegate Generator ConsoleGeneratorFactory(Platform platform, string directory);