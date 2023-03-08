﻿#pragma warning disable CS1591
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using DearImGui.Extensions;
using DearImGui.OpenTK.Extensions;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PInvoke;
using Image = OpenTK.Windowing.GraphicsLibraryFramework.Image;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using Vector2 = System.Numerics.Vector2;
using Win32Exception = PInvoke.Win32Exception;

// ReSharper disable InconsistentNaming

namespace DearImGui.OpenTK;

/// <summary>
///     OpenTK controller for imgui.
/// </summary>
public sealed class ImGuiController : Disposable
{
    private const string ShaderSourceVS = @"
        #version 330 core

        uniform mat4 Projection;

        layout(location = 0) in vec2 Position;
        layout(location = 1) in vec2 UV;
        layout(location = 2) in vec4 Color;

        out vec4 Frag_Color;
        out vec2 Frag_UV;

        void main()
        {
            gl_Position = Projection * vec4(Position, 0, 1);
            Frag_Color = Color;
            Frag_UV = UV;
        }";

    private const string ShaderSourceFS = @"
        #version 330 core

        uniform sampler2D Texture;

        in vec4 Frag_Color;
        in vec2 Frag_UV;

        layout(location = 0) out vec4 Out_Color;

        void main()
        {
            Out_Color = Frag_Color * texture(Texture, Frag_UV);
        }
        ";

    private static readonly Dictionary<Keys, ImGuiKey> KeyMap = new()
    {
        { Keys.Tab, ImGuiKey.Tab },
        { Keys.Left, ImGuiKey.LeftArrow },
        { Keys.Right, ImGuiKey.RightArrow },
        { Keys.Up, ImGuiKey.UpArrow },
        { Keys.Down, ImGuiKey.DownArrow },
        { Keys.PageUp, ImGuiKey.PageUp },
        { Keys.PageDown, ImGuiKey.PageDown },
        { Keys.Home, ImGuiKey.Home },
        { Keys.End, ImGuiKey.End },
        { Keys.Insert, ImGuiKey.Insert },
        { Keys.Delete, ImGuiKey.Delete },
        { Keys.Backspace, ImGuiKey.Backspace },
        { Keys.Space, ImGuiKey.Space },
        { Keys.Enter, ImGuiKey.Enter },
        { Keys.Escape, ImGuiKey.Escape },
        { Keys.Apostrophe, ImGuiKey.Apostrophe },
        { Keys.Comma, ImGuiKey.Comma },
        { Keys.Minus, ImGuiKey.Minus },
        { Keys.Period, ImGuiKey.Period },
        { Keys.Slash, ImGuiKey.Slash },
        { Keys.Semicolon, ImGuiKey.Semicolon },
        { Keys.Equal, ImGuiKey.Equal },
        { Keys.LeftBracket, ImGuiKey.LeftBracket },
        { Keys.Backslash, ImGuiKey.Backslash },
        { Keys.RightBracket, ImGuiKey.RightBracket },
        { Keys.GraveAccent, ImGuiKey.GraveAccent },
        { Keys.CapsLock, ImGuiKey.CapsLock },
        { Keys.ScrollLock, ImGuiKey.ScrollLock },
        { Keys.NumLock, ImGuiKey.NumLock },
        { Keys.PrintScreen, ImGuiKey.PrintScreen },
        { Keys.Pause, ImGuiKey.Pause },
        { Keys.KeyPad0, ImGuiKey.Keypad0 },
        { Keys.KeyPad1, ImGuiKey.Keypad1 },
        { Keys.KeyPad2, ImGuiKey.Keypad2 },
        { Keys.KeyPad3, ImGuiKey.Keypad3 },
        { Keys.KeyPad4, ImGuiKey.Keypad4 },
        { Keys.KeyPad5, ImGuiKey.Keypad5 },
        { Keys.KeyPad6, ImGuiKey.Keypad6 },
        { Keys.KeyPad7, ImGuiKey.Keypad7 },
        { Keys.KeyPad8, ImGuiKey.Keypad8 },
        { Keys.KeyPad9, ImGuiKey.Keypad9 },
        { Keys.KeyPadDecimal, ImGuiKey.KeypadDecimal },
        { Keys.KeyPadDivide, ImGuiKey.KeypadDivide },
        { Keys.KeyPadMultiply, ImGuiKey.KeypadMultiply },
        { Keys.KeyPadSubtract, ImGuiKey.KeypadSubtract },
        { Keys.KeyPadAdd, ImGuiKey.KeypadAdd },
        { Keys.KeyPadEnter, ImGuiKey.KeypadEnter },
        { Keys.KeyPadEqual, ImGuiKey.KeypadEqual },
        { Keys.LeftShift, ImGuiKey.LeftShift },
        { Keys.LeftControl, ImGuiKey.LeftCtrl },
        { Keys.LeftAlt, ImGuiKey.LeftAlt },
        { Keys.LeftSuper, ImGuiKey.LeftSuper },
        { Keys.RightShift, ImGuiKey.RightShift },
        { Keys.RightControl, ImGuiKey.RightCtrl },
        { Keys.RightAlt, ImGuiKey.RightAlt },
        { Keys.RightSuper, ImGuiKey.RightSuper },
        { Keys.Menu, ImGuiKey.Menu },
        { Keys.D0, ImGuiKey.D0 },
        { Keys.D1, ImGuiKey.D1 },
        { Keys.D2, ImGuiKey.D2 },
        { Keys.D3, ImGuiKey.D3 },
        { Keys.D4, ImGuiKey.D4 },
        { Keys.D5, ImGuiKey.D5 },
        { Keys.D6, ImGuiKey.D6 },
        { Keys.D7, ImGuiKey.D7 },
        { Keys.D8, ImGuiKey.D8 },
        { Keys.D9, ImGuiKey.D9 },
        { Keys.A, ImGuiKey.A },
        { Keys.B, ImGuiKey.B },
        { Keys.C, ImGuiKey.C },
        { Keys.D, ImGuiKey.D },
        { Keys.E, ImGuiKey.E },
        { Keys.F, ImGuiKey.F },
        { Keys.G, ImGuiKey.G },
        { Keys.H, ImGuiKey.H },
        { Keys.I, ImGuiKey.I },
        { Keys.J, ImGuiKey.J },
        { Keys.K, ImGuiKey.K },
        { Keys.L, ImGuiKey.L },
        { Keys.M, ImGuiKey.M },
        { Keys.N, ImGuiKey.N },
        { Keys.O, ImGuiKey.O },
        { Keys.P, ImGuiKey.P },
        { Keys.Q, ImGuiKey.Q },
        { Keys.R, ImGuiKey.R },
        { Keys.S, ImGuiKey.S },
        { Keys.T, ImGuiKey.T },
        { Keys.U, ImGuiKey.U },
        { Keys.V, ImGuiKey.V },
        { Keys.W, ImGuiKey.W },
        { Keys.X, ImGuiKey.X },
        { Keys.Y, ImGuiKey.Y },
        { Keys.Z, ImGuiKey.Z },
        { Keys.F1, ImGuiKey.F1 },
        { Keys.F2, ImGuiKey.F2 },
        { Keys.F3, ImGuiKey.F3 },
        { Keys.F4, ImGuiKey.F4 },
        { Keys.F5, ImGuiKey.F5 },
        { Keys.F6, ImGuiKey.F6 },
        { Keys.F7, ImGuiKey.F7 },
        { Keys.F8, ImGuiKey.F8 },
        { Keys.F9, ImGuiKey.F9 },
        { Keys.F10, ImGuiKey.F10 },
        { Keys.F11, ImGuiKey.F11 },
        { Keys.F12, ImGuiKey.F12 }
    };

    private readonly ImGuiIO IO;

    private readonly Dictionary<ImGuiMouseCursor, IntPtr> MouseCursors;

    private readonly GameWindow Window;

    private ImDrawList[] DrawLists = new ImDrawList[100];

    private int IndexBuffer;

    private int IndexBufferSize = 2000;

    private bool NewFrame;

    private int Shader;

    private int ShaderProjection;

    private int ShaderTexture;

    private int Texture;

    private int VertexArray;

    private int VertexBuffer;

    private int VertexBufferSize = 10000;

    private ImGuiControllerWindowEvents WindowEvents_ = ImGuiControllerWindowEvents.Everything;

    public ImGuiController(GameWindow window, string? fontPath = null, float? fontSize = null)
    {
        Window = window ?? throw new ArgumentNullException(nameof(window));

        ImGui.SetCurrentContext(ImGui.CreateContext());

        IO = ImGui.GetIO(); // needs context

        Context = ImGui.GetCurrentContext();

        using (new ImGuiContextScope(Context))
        {
            InitializeFlags();

            InitializeFont(fontPath, fontSize);

            InitializeStyle();
        }

        InitializeBuffers();

        InitializeShader();

        MouseCursors = CreateMouseCursors();

        Window.FocusedChanged += OnWindowFocusChanged;
        Window.KeyDown += OnWindowKeyPressDown;
        Window.KeyUp += OnWindowKeyPressUp;
        Window.MouseEnter += OnWindowMouseEnter;
        Window.MouseMove += OnWindowMouseMove;
        Window.MouseDown += OnWindowMouseButtonDown;
        Window.MouseUp += OnWindowMouseButtonUp;
        Window.MouseWheel += OnWindowMouseWheel;
        Window.TextInput += OnWindowTextInput;
    }

    /// <summary>
    ///     Gets the imgui context for this instance.
    /// </summary>
    public IntPtr Context { get; }

    /// <summary>
    ///     Gets or sets which window events this instance should listen to.
    /// </summary>
    public ref ImGuiControllerWindowEvents WindowEvents => ref WindowEvents_;

    /// <inheritdoc />
    protected override void DisposeManaged()
    {
        Window.FocusedChanged -= OnWindowFocusChanged;
        Window.KeyDown -= OnWindowKeyPressDown;
        Window.KeyUp -= OnWindowKeyPressUp;
        Window.MouseEnter -= OnWindowMouseEnter;
        Window.MouseMove -= OnWindowMouseMove;
        Window.MouseDown -= OnWindowMouseButtonDown;
        Window.MouseUp -= OnWindowMouseButtonUp;
        Window.MouseWheel -= OnWindowMouseWheel;
        Window.TextInput -= OnWindowTextInput;

        IO.Dispose();

        ImGui.DestroyContext(Context);

        GL.DeleteVertexArray(VertexArray);

        GL.DeleteBuffer(VertexBuffer);

        GL.DeleteBuffer(IndexBuffer);

        GL.DeleteProgram(Shader);

        GL.DeleteTexture(Texture);

        base.DisposeManaged();
    }

    private static int CreateProgram(string name, string sourceVS, string sourceFS)
    {
        const string label = "ImGui program";

        var program = GL.CreateProgram();

        GL.ObjectLabel(ObjectLabelIdentifier.Program, program, label.Length, label);

        var vs = CreateShader(name, ShaderType.VertexShader, sourceVS);
        var fs = CreateShader(name, ShaderType.FragmentShader, sourceFS);

        GL.AttachShader(program, vs);
        GL.AttachShader(program, fs);

        GL.LinkProgram(program);

        GL.DetachShader(program, vs);
        GL.DetachShader(program, fs);

        GL.DeleteShader(vs);
        GL.DeleteShader(fs);

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var success);

        if (success != 0)
        {
            return program;
        }

        var log = GL.GetProgramInfoLog(program);

        throw new InvalidOperationException($"Failed to create program:\n{log}");
    }

    private static int CreateShader(string name, ShaderType type, string source)
    {
        var shader = GL.CreateShader(type);

        GL.ObjectLabel(ObjectLabelIdentifier.Shader, shader, name.Length, name);

        GL.ShaderSource(shader, source);

        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out var success);

        if (success != 0)
        {
            return shader;
        }

        var log = GL.GetShaderInfoLog(shader);

        throw new InvalidOperationException($"Failed to create shader:\n{log}");
    }

    private void InitializeFlags()
    {
        if (Window.APIVersion >= new Version(3, 2))
        {
            IO.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
        }

        IO.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;

        IO.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        IO.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
    }

    private unsafe void InitializeFont(string? fontPath, float? fontSize)
    {
        if (fontPath != null)
        {
            fontSize = fontSize switch
            {
                null => 10.0f,
                <= 0.0f => throw new ArgumentOutOfRangeException(nameof(fontSize), fontSize, null),
                _ => fontSize
            };

            if (!File.Exists(fontPath))
            {
                throw new FileNotFoundException("The font file could not be found.", fontPath);
            }

            var size = GetDpiScaledFontSize(fontSize!.Value);

            var ranges = IO.Fonts.GlyphRangesDefault;

            IO.Fonts.AddFontFromFileTTF(fontPath, size, null, ref *ranges);
        }
        else
        {
            IO.Fonts.AddFontDefault();
        }

        UpdateFontsTextureAtlas();
    }

    private unsafe void InitializeStyle()
    {
        var style = ImGui.GetStyle();

        ImGui.StyleColorsDark(style);

        GLFW.GetWindowContentScale(Window.WindowPtr, out var xScale, out var yScale);

        var scale = 1.0f;

        if (Math.Abs(xScale - yScale) < 0.001f)
        {
            scale = xScale;
        }

        style.ScaleAllSizes(scale);
    }

    private void InitializeBuffers()
    {
        const string labelVBO = "ImGui VBO";
        const string labelIBO = "ImGui IBO";
        const string labelVAO = "ImGui VAO";

        var sizeOfIBO = Marshal.SizeOf<short>();
        var sizeOfVBO = Marshal.SizeOf<ImDrawVert>();

        GL.CreateBuffers(1, out VertexBuffer);
        GL.ObjectLabel(ObjectLabelIdentifier.Buffer, VertexBuffer, labelVBO.Length, labelVBO);
        GL.NamedBufferData(VertexBuffer, VertexBufferSize * sizeOfVBO, IntPtr.Zero, BufferUsageHint.StreamDraw);

        GL.CreateBuffers(1, out IndexBuffer);
        GL.ObjectLabel(ObjectLabelIdentifier.Buffer, IndexBuffer, labelIBO.Length, labelIBO);
        GL.NamedBufferData(IndexBuffer, IndexBufferSize * sizeOfIBO, IntPtr.Zero, BufferUsageHint.StreamDraw);

        GL.CreateVertexArrays(1, out VertexArray);
        GL.ObjectLabel(ObjectLabelIdentifier.VertexArray, VertexArray, labelVAO.Length, labelVAO);

        GL.VertexArrayVertexBuffer(VertexArray, 0, VertexBuffer, IntPtr.Zero, sizeOfVBO);
        GL.VertexArrayElementBuffer(VertexArray, IndexBuffer);

        GL.EnableVertexArrayAttrib(VertexArray, 0);
        GL.EnableVertexArrayAttrib(VertexArray, 1);
        GL.EnableVertexArrayAttrib(VertexArray, 2);

        var offset1 = Marshal.OffsetOf<ImDrawVert>(nameof(ImDrawVert.Pos)).ToInt32();
        var offset2 = Marshal.OffsetOf<ImDrawVert>(nameof(ImDrawVert.Uv)).ToInt32();
        var offset3 = Marshal.OffsetOf<ImDrawVert>(nameof(ImDrawVert.Col)).ToInt32();

        GL.VertexArrayAttribFormat(VertexArray, 0, 2, VertexAttribType.Float, false, offset1);
        GL.VertexArrayAttribFormat(VertexArray, 1, 2, VertexAttribType.Float, false, offset2);
        GL.VertexArrayAttribFormat(VertexArray, 2, 4, VertexAttribType.UnsignedByte, true, offset3);

        GL.VertexArrayAttribBinding(VertexArray, 0, 0);
        GL.VertexArrayAttribBinding(VertexArray, 1, 0);
        GL.VertexArrayAttribBinding(VertexArray, 2, 0);
    }

    private void InitializeShader()
    {
        Shader = CreateProgram("ImGui shader", ShaderSourceVS, ShaderSourceFS);
        ShaderProjection = GL.GetUniformLocation(Shader, "Projection");
        ShaderTexture = GL.GetUniformLocation(Shader, "Texture");
    }

    private void OnWindowFocusChanged(FocusedChangedEventArgs e)
    {
        if (!WindowEvents.HasFlags(ImGuiControllerWindowEvents.FocusedChanged))
            return;

        IO.AddFocusEvent(e.IsFocused);
    }

    private void OnWindowKeyPress(KeyboardKeyEventArgs e, bool pressed)
    {
        if (e.IsRepeat)
        {
            return;
        }

        UpdateModifiers(e.Modifiers);

        if (KeyMap.TryGetValue(e.Key, out var key))
        {
            IO.AddKeyEvent(key, pressed);
        }
    }

    private void OnWindowKeyPressDown(KeyboardKeyEventArgs e)
    {
        if (!WindowEvents.HasFlags(ImGuiControllerWindowEvents.KeyDown))
            return;

        OnWindowKeyPress(e, true);
    }

    private void OnWindowKeyPressUp(KeyboardKeyEventArgs e)
    {
        if (!WindowEvents.HasFlags(ImGuiControllerWindowEvents.KeyUp))
            return;

        OnWindowKeyPress(e, false);
    }

    private void OnWindowMouseEnter()
    {
        if (!WindowEvents.HasFlags(ImGuiControllerWindowEvents.MouseEnter))
            return;

        var position = Window.MouseState.Position;

        IO.AddMousePosEvent(position.X, position.Y);
    }

    private void OnWindowMouseMove(MouseMoveEventArgs e)
    {
        if (!WindowEvents.HasFlags(ImGuiControllerWindowEvents.MouseMove))
            return;

        IO.AddMousePosEvent(e.X, e.Y);
    }

    private void OnWindowMouseButton(MouseButtonEventArgs e)
    {
        UpdateModifiers(e.Modifiers);

        var button = (int)e.Button;

        if (button is >= 0 and < 3)
        {
            IO.AddMouseButtonEvent(button, e.IsPressed);
        }
    }

    private void OnWindowMouseButtonDown(MouseButtonEventArgs e)
    {
        if (!WindowEvents.HasFlags(ImGuiControllerWindowEvents.MouseDown))
            return;

        OnWindowMouseButton(e);
    }

    private void OnWindowMouseButtonUp(MouseButtonEventArgs e)
    {
        if (!WindowEvents.HasFlags(ImGuiControllerWindowEvents.MouseUp))
            return;

        OnWindowMouseButton(e);
    }

    private void OnWindowMouseWheel(MouseWheelEventArgs e)
    {
        if (!WindowEvents.HasFlags(ImGuiControllerWindowEvents.MouseWheel))
            return;

        IO.AddMouseWheelEvent(e.OffsetX, e.OffsetY);
    }

    private void OnWindowTextInput(TextInputEventArgs e)
    {
        if (!WindowEvents.HasFlags(ImGuiControllerWindowEvents.TextInput))
            return;

        IO.AddInputCharacter((uint)e.Unicode);
    }

    /// <summary>
    ///     Scales a font size according current environment's DPI.
    /// </summary>
    /// <param name="size">
    ///     The input font size.
    /// </param>
    /// <returns>
    ///     The scaled font size.
    /// </returns>
    public float GetDpiScaledFontSize(float size)
    {
        float xScale;
        float yScale;

        unsafe
        {
            GLFW.GetWindowContentScale(Window.WindowPtr, out xScale, out yScale);
        }

        var equal = Math.Abs(xScale - yScale) < 0.001f;

        // the above condition should always be true but who knows...

        Trace.Assert(equal, $"{nameof(GetDpiScaledFontSize)}: mismatch between {nameof(xScale)} and {nameof(yScale)}");

        var scale = equal ? xScale : 1.0f;

        // whatever that formula is, it just looks like as in Notepad

        return size * scale * scale;
    }

    /// <summary>
    ///     Updates the fonts texture atlas.
    /// </summary>
    /// <remarks>
    ///     To add an additional font, proceed as follows:
    ///     <list type="bullet">
    ///         <item>
    ///             add a font using a function from <see cref="ImFontAtlas" />, e.g.
    ///             <see cref="ImFontAtlas.AddFontFromFileTTF(string,float,DearImGui.ImFontConfig,ref ushort)" />
    ///         </item>
    ///         <item>
    ///             call <see cref="ImFontAtlas.Build" /> to build pixel data
    ///         </item>
    ///         <item>
    ///             call this method to update the fonts texture atlas
    ///         </item>
    ///     </list>
    ///     This cannot be done between <see cref="ImGui.NewFrame" /> and <see cref="ImGui.EndFrame" />,
    ///     e.g. do that before calling <see cref="Update" />.
    /// </remarks>
    public unsafe void UpdateFontsTextureAtlas()
    {
        if (GL.IsTexture(Texture))
        {
            GL.DeleteTextures(1, ref Texture);
        }

        var pp = new IntPtr();
        var pw = default(int);
        var ph = default(int);
        var ps = default(int);

        var pointer = Unsafe.AsPointer(ref pp);

        IO.Fonts.GetTexDataAsRGBA32((byte**)pointer, ref pw, ref ph, ref ps);

        const string labelTEX = "ImGui Texture";
        GL.CreateTextures(TextureTarget.Texture2D, 1, out Texture);
        GL.ObjectLabel(ObjectLabelIdentifier.Texture, Texture, labelTEX.Length, labelTEX);

        var levels = (int)Math.Floor(Math.Log(Math.Max(pw, ph), 2));

        GL.TextureParameter(Texture, TextureParameterName.TextureMaxLevel, levels - 1);
        GL.TextureParameter(Texture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TextureParameter(Texture, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TextureParameter(Texture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TextureParameter(Texture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.TextureStorage2D(Texture, levels, SizedInternalFormat.Rgba8, pw, ph);

        GL.TextureSubImage2D(Texture, 0, 0, 0, pw, ph, PixelFormat.Bgra, PixelType.UnsignedByte, pp);

        GL.GenerateTextureMipmap(Texture);

        IO.Fonts.TexID = (IntPtr)Texture;

        IO.Fonts.ClearTexData();
    }

    /// <summary>
    ///     Updates this instance.
    /// </summary>
    /// <param name="deltaTime">
    ///     The time that passed since last frame.
    /// </param>
    public void Update(float deltaTime)
    {
        using (new ImGuiContextScope(Context))
        {
            var size = Window.ClientSize;

            IO.DisplaySize = new Vector2(size.X, size.Y);

            IO.DeltaTime = deltaTime;

            UpdateMouseCursor();

            ImGui.NewFrame();

            NewFrame = true;
        }
    }

    private void UpdateModifiers(KeyModifiers modifiers)
    {
        var control = modifiers.HasFlags(KeyModifiers.Control);
        var shift = modifiers.HasFlags(KeyModifiers.Shift);
        var alt = modifiers.HasFlags(KeyModifiers.Alt);
        var super = modifiers.HasFlags(KeyModifiers.Super);

        IO.AddKeyEvent(ImGuiKey.ModCtrl, control);
        IO.AddKeyEvent(ImGuiKey.ModShift, shift);
        IO.AddKeyEvent(ImGuiKey.ModAlt, alt);
        IO.AddKeyEvent(ImGuiKey.ModSuper, super);
    }

    private void UpdateMouseCursor()
    {
        if ((IO.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) != ImGuiConfigFlags.None)
        {
            return;
        }

        var bounds = Window.Bounds;

        if (bounds.ContainsExclusive(bounds.Min + (Vector2i)Window.MouseState.Position) is false)
        {
            return; // but let system update cursor when mouse is outside, i.e. when resizing
        }

        var cursor = ImGui.GetMouseCursor();

        if (IO.MouseDrawCursor)
        {
            cursor = ImGuiMouseCursor.None;
        }

        unsafe
        {
            GLFW.SetCursor(Window.WindowPtr, (Cursor*)MouseCursors[cursor].ToPointer());
        }
    }

    private static Dictionary<ImGuiMouseCursor, IntPtr> CreateMouseCursors()
    {
        var dictionary = new Dictionary<ImGuiMouseCursor, IntPtr>();

        var cursors = new[]
        {
            ImGuiMouseCursor.Arrow,
            ImGuiMouseCursor.Hand,
            ImGuiMouseCursor.NotAllowed,
            ImGuiMouseCursor.ResizeAll,
            ImGuiMouseCursor.ResizeEW,
            ImGuiMouseCursor.ResizeNESW,
            ImGuiMouseCursor.ResizeNS,
            ImGuiMouseCursor.ResizeNWSE,
            ImGuiMouseCursor.TextInput
        };

        foreach (var cursor in cursors)
        {
            if (OperatingSystem.IsWindows())
            {
                unsafe
                {
                    var handle = CreateMouseCursorWindows(cursor);

                    dictionary.Add(cursor, new IntPtr(handle));
                }
            }
        }

        return dictionary;
    }

    [SupportedOSPlatform("windows")]
    private static unsafe Cursor* CreateMouseCursorWindows(ImGuiMouseCursor mouseCursor)
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault

        var resourceId = mouseCursor switch
        {
            ImGuiMouseCursor.Arrow => User32.Cursors.IDC_ARROW,
            ImGuiMouseCursor.TextInput => User32.Cursors.IDC_IBEAM,
            ImGuiMouseCursor.ResizeAll => User32.Cursors.IDC_SIZEALL,
            ImGuiMouseCursor.ResizeNS => User32.Cursors.IDC_SIZENS,
            ImGuiMouseCursor.ResizeEW => User32.Cursors.IDC_SIZEWE,
            ImGuiMouseCursor.ResizeNESW => User32.Cursors.IDC_SIZENESW,
            ImGuiMouseCursor.ResizeNWSE => User32.Cursors.IDC_SIZENWSE,
            ImGuiMouseCursor.Hand => User32.Cursors.IDC_HAND,
            ImGuiMouseCursor.NotAllowed => User32.Cursors.IDC_NO,
            _ => throw new InvalidEnumArgumentException(null, (int)mouseCursor, typeof(ImGuiMouseCursor))
        };

        using (var hCursor = User32.LoadCursor(IntPtr.Zero, Kernel32.MAKEINTRESOURCE((int)resourceId)))
        using (var hIcon = Icon.FromHandle(hCursor.DangerousGetHandle()))
        using (var bitmap = hIcon.ToBitmap())
        {
            if (!User32.GetIconInfo(hIcon.Handle, out var iconInfo))
            {
                throw new Win32Exception();
            }

            var data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var source = (data.Scan0 + (y * data.Stride + x * 4)).ToPointer();
                    var color1 = Color.FromArgb(Unsafe.Read<int>(source));
                    var color2 = Color.FromArgb(color1.A, color1.B, color1.G, color1.R);
                    Unsafe.Write(source, color2.ToArgb());
                }
            }

            var image = new Image(bitmap.Width, bitmap.Height, (byte*)data.Scan0.ToPointer());

            var cursor = GLFW.CreateCursor(image, iconInfo.xHotspot, iconInfo.yHotspot);

            bitmap.UnlockBits(data);

            return cursor;
        }
    }

    /// <summary>
    ///     Renders this instance.
    /// </summary>
    public void Render()
    {
        if (NewFrame is false)
        {
            return;
        }

        ImGui.EndFrame();
        ImGui.Render();

        RenderFrame();

        NewFrame = false;
    }

    private unsafe void RenderFrame()
    {
        var data = ImGui.GetDrawData();

        if (data.CmdListsCount == 0)
        {
            return;
        }

        var scaleX = (int)(data.DisplaySize.X * data.FramebufferScale.X);
        var scaleY = (int)(data.DisplaySize.Y * data.FramebufferScale.Y);

        if (scaleX <= 0 || scaleY <= 0)
        {
            return;
        }

        var projection = Matrix4.CreateOrthographicOffCenter(0.0f, IO.DisplaySize.X, IO.DisplaySize.Y, 0.0f, -1.0f, 1.0f);

        using var scope = new OpenGLStateScope();

        GL.Enable(EnableCap.Blend);
        GL.BlendEquation(BlendEquationMode.FuncAdd);
        GL.Enable(EnableCap.ScissorTest);
        GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);
        GL.Disable(EnableCap.StencilTest);
        GL.Enable(EnableCap.ScissorTest);
        GL.Disable(EnableCap.PrimitiveRestart);
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        GL.UseProgram(Shader);
        GL.Uniform1(ShaderTexture, 0);
        GL.UniformMatrix4(ShaderProjection, false, ref projection);
        GL.BindSampler(0, 0);
        GL.BindVertexArray(VertexArray);

        var count = data.GetCmdLists(ref DrawLists);

        for (var i = 0; i < count; i++)
        {
            var list = DrawLists[i];

            var vtxBuffer = list.VtxBuffer;
            var idxBuffer = list.IdxBuffer;

            var vtxBufferSize = vtxBuffer.Size * sizeof(ImDrawVert);
            var idxBufferSize = idxBuffer.Size * sizeof(ushort);

            if (VertexBufferSize < vtxBufferSize)
            {
                VertexBufferSize = vtxBufferSize;
                GL.NamedBufferData(VertexBuffer, VertexBufferSize, IntPtr.Zero, BufferUsageHint.StreamDraw);
            }

            if (IndexBufferSize < idxBufferSize)
            {
                IndexBufferSize = idxBufferSize;
                GL.NamedBufferData(IndexBuffer, IndexBufferSize, IntPtr.Zero, BufferUsageHint.StreamDraw);
            }

            GL.NamedBufferSubData(VertexBuffer, IntPtr.Zero, vtxBufferSize, vtxBuffer.Data);
            GL.NamedBufferSubData(IndexBuffer, IntPtr.Zero, idxBufferSize, idxBuffer.Data);

            for (var j = 0; j < list.CmdBuffer.Size; j++)
            {
                var cmd = list.CmdBuffer[j];

                var clipOff = data.DisplayPos;
                var clipScl = data.FramebufferScale;
                var clipMin = new Vector2((cmd.ClipRect.X - clipOff.X) * clipScl.X, (cmd.ClipRect.Y - clipOff.Y) * clipScl.Y);
                var clipMax = new Vector2((cmd.ClipRect.Z - clipOff.X) * clipScl.X, (cmd.ClipRect.W - clipOff.Y) * clipScl.Y);

                if (clipMax.X <= clipMin.X || clipMax.Y <= clipMin.Y)
                {
                    continue;
                }

                if (cmd.UserCallback != null)
                {
                    throw new NotImplementedException(); // TODO see imgui.h
                }

                GL.Scissor((int)clipMin.X, (int)(scaleY - clipMax.Y), (int)(clipMax.X - clipMin.X), (int)(clipMax.Y - clipMin.Y));

                GL.BindTextureUnit(0, cmd.GetTexID().ToInt32());

                var length = (int)cmd.ElemCount;
                var offset = cmd.IdxOffset * sizeof(ushort);

                if ((IO.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != ImGuiBackendFlags.None)
                {
                    GL.DrawElementsBaseVertex(PrimitiveType.Triangles, length, DrawElementsType.UnsignedShort, (IntPtr)offset, unchecked((int)cmd.VtxOffset));
                }
                else
                {
                    GL.DrawElements(BeginMode.Triangles, length, DrawElementsType.UnsignedShort, (int)offset);
                }
            }
        }
    }
}