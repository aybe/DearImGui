using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace imgui.NET.OpenTK.Test;

/// <summary>
///     A game window that uses OpenGL 4.5 with an enabled debug context.
/// </summary>
public abstract class GameWindowBaseWithDebugContext : GameWindowBase
    // https://www.khronos.org/opengl/wiki/OpenGL_Context
    // https://www.khronos.org/opengl/wiki/Debug_Output
    // https://learnopengl.com/In-Practice/Debugging
    // TODO filtering, toggling, etc
{
    private readonly DebugProc DebugProc = DebugProcCallback;

    protected GameWindowBaseWithDebugContext(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, UpdateSettings(nativeWindowSettings))
    {
        {
            GL.GetInteger(GetPName.ContextFlags, out var data);

            if ((data & (int)ContextFlags.Debug) != 0)
            {
                // BUG/TODO it can fail but debugging still be enabled
            }
        }

        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        GL.DebugMessageCallback(DebugProc, IntPtr.Zero);
        GL.DebugMessageControl(DebugSourceControl.DontCare, DebugTypeControl.DontCare, DebugSeverityControl.DontCare, 0, Array.Empty<int>(), true);
    }

    private static NativeWindowSettings UpdateSettings(NativeWindowSettings settings)
    {
        settings.APIVersion = new Version(4, 5);

        settings.Flags |= ContextFlags.Debug | ContextFlags.ForwardCompatible;

        return settings;
    }

    [DebuggerNonUserCode]
    private static void DebugProcCallback(
        DebugSource source,
        DebugType type,
        int id,
        DebugSeverity severity,
        int length,
        IntPtr message,
        IntPtr userParam)
    {
        var msg = Marshal.PtrToStringAnsi(message);

        var builder = new StringBuilder();

        builder.AppendLine($"Source: {source}");
        builder.AppendLine($"Type: {type}");
        builder.AppendLine($"Id: {id}");
        builder.AppendLine($"Severity: {severity}");
        builder.AppendLine($"Message: {msg}");

        var str = builder.ToString();

        throw new InvalidOperationException(str);
    }
}