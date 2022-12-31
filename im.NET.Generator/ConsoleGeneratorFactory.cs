using System.Runtime.InteropServices;

namespace im.NET.Generator;

public delegate ConsoleGenerator ConsoleGeneratorFactory(Architecture architecture, string directory);