using Microsoft.CodeAnalysis;

namespace im.NET.Generator;

public delegate ConsoleGenerator ConsoleGeneratorFactory(Platform platform, string directory);