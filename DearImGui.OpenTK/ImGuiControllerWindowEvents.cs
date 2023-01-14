using OpenTK.Windowing.Desktop;

#pragma warning disable CS1591
namespace DearImGui.OpenTK;

/// <summary>
///     Specifies which window events a controller should listen to.
/// </summary>
[Flags]
public enum ImGuiControllerWindowEvents
{
    /// <summary>
    ///     Listen to none of the window event specified in <see cref="ImGuiControllerWindowEvents" />.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Listen to every window event specified in <see cref="ImGuiControllerWindowEvents" />.
    /// </summary>
    Everything = ~None,

    /// <summary>
    ///     Listen to <see cref="NativeWindow.FocusedChanged" />.
    /// </summary>
    FocusedChanged = 1 << 0,

    /// <summary>
    ///     Listen to <see cref="NativeWindow.KeyDown" />.
    /// </summary>
    KeyDown = 1 << 1,

    /// <summary>
    ///     Listen to <see cref="NativeWindow.KeyUp" />.
    /// </summary>
    KeyUp = 1 << 2,

    /// <summary>
    ///     Listen to <see cref="NativeWindow.MouseDown" />.
    /// </summary>
    MouseDown = 1 << 3,

    /// <summary>
    ///     Listen to <see cref="NativeWindow.MouseUp" />.
    /// </summary>
    MouseUp = 1 << 4,

    /// <summary>
    ///     Listen to <see cref="NativeWindow.MouseEnter" />.
    /// </summary>
    MouseEnter = 1 << 5,

    /// <summary>
    ///     Listen to <see cref="NativeWindow.MouseMove" />.
    /// </summary>
    MouseMove = 1 << 6,

    /// <summary>
    ///     Listen to <see cref="NativeWindow.MouseWheel" />.
    /// </summary>
    MouseWheel = 1 << 7,

    /// <summary>
    ///     Listen to <see cref="NativeWindow.TextInput" />.
    /// </summary>
    TextInput = 1 << 8
}