using Microsoft.CodeAnalysis;

namespace DearGenerator;

public delegate CodeGenerator CodeGeneratorFactory(Platform platform, string directory);