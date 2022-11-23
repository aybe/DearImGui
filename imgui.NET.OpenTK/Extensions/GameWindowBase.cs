using System.ComponentModel;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace imgui.NET.OpenTK.Extensions;

/// <summary>
///     Base class for a game window.
/// </summary>
public abstract class GameWindowBase : GameWindow
{
    /// <inheritdoc />
    protected GameWindowBase(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
    }

    private bool WindowWasMaximized { get; set; }

    /// <inheritdoc />
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.ClearColor(Color.CornflowerBlue);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        Context.SwapBuffers();

        base.OnRenderFrame(args);
    }

    /// <inheritdoc />
    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);

        base.OnResize(e);
    }

    /// <inheritdoc />
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        var alt = KeyboardState.IsKeyDown(Keys.LeftAlt, Keys.RightAlt);
        var ret = KeyboardState.IsKeyPressed(Keys.Enter, Keys.KeyPadEnter);

        if (alt && ret)
        {
            ToggleFullScreen();
        }

        base.OnUpdateFrame(args);
    }

    private void ToggleFullScreen()
    {
        // we also want to transition from/to maximized/full screen but it's buggy, fix that

        var maximized = WindowState is WindowState.Maximized;

        switch (WindowState)
        {
            case WindowState.Normal:
                WindowState = WindowState.Fullscreen;
                break;
            case WindowState.Minimized:
                WindowState = WindowState.Minimized;
                break;
            case WindowState.Maximized:
                WindowState = WindowState.Normal;
                WindowState = WindowState.Fullscreen;
                break;
            case WindowState.Fullscreen:
                if (WindowWasMaximized)
                {
                    WindowState = WindowState.Normal;
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    WindowState = WindowState.Normal;
                }
                break;
            default:
                throw new InvalidEnumArgumentException(null, (int)WindowState, typeof(WindowState));
        }

        WindowWasMaximized = maximized;
    }
}