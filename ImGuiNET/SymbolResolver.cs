using System.Runtime.InteropServices;
using JetBrains.Annotations;

// ReSharper disable StringLiteralTypo

namespace ImGuiNET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class SymbolResolver
{
    public static IntPtr LoadImage(ref string path)
    {
        string directory;

        if (OperatingSystem.IsWindows())
        {
            directory = Environment.Is64BitProcess ? "win-x64" : "win-x86";
        }
        else
        {
            throw new NotSupportedException();
        }

        string extension;

        if (OperatingSystem.IsWindows())
        {
            extension = "dll";
        }
        else
        {
            throw new NotSupportedException();
        }

        path = Path.Combine(Environment.CurrentDirectory, "runtimes", directory, "native", path);

        path = Path.ChangeExtension(path, extension);

        var load = NativeLibrary.Load(path);

        return load;
    }

    public static IntPtr ResolveSymbol(IntPtr handle, string name)
    {
        var export = NativeLibrary.GetExport(handle, name);

        return export;
    }
}