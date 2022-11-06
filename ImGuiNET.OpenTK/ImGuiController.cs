// #define GLFW_MOUSE_CURSOR_WINDOWS

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector2 = System.Numerics.Vector2;

// ReSharper disable InconsistentNaming

namespace ImGuiNET.OpenTK;

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

    private readonly ImGuiContext Context;

    //private readonly IntPtr ContextPlot = ImPlot.CreateContext();

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

    public ImGuiController(GameWindow window, ImGuiFontConfig? fontConfig = null)
    {
        Window = window;

        ImGui.SetCurrentContext(ImGui.CreateContext(null));

        Context = ImGui.GetCurrentContext();

        using (new ImGuiContextScope(Context))
        {
            var io = IO;

            InitializeFlags(io);

            InitializeFont(io, fontConfig);

            InitializeStyle();
        }

        InitializeBuffers();

        InitializeShader();

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

    private ImGuiIO IO => ImGui.GetIO();

    private GameWindow Window { get; }

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

        ImGui.DestroyContext(Context);

        //ImPlot.DestroyContext(ContextPlot);

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

    private void InitializeFlags(ImGuiIO io)
    {
        if (Window.APIVersion >= new Version(3, 2))
        {
            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
        }

        //io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors; // todo

        //io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
    }

    private unsafe void InitializeFont(ImGuiIO io, ImGuiFontConfig? fontConfig)
    {
        if (fontConfig.HasValue)
        {
            var scale = 1.0f;

            GLFW.GetWindowContentScale(Window.WindowPtr, out var xScale, out var yScale);

            if (Math.Abs(xScale - yScale) < 0.001f)
            {
                scale = xScale;
            }

            var size = fontConfig.Value.Size * scale * scale; // so it looks the same as in Notepad

            var ranges = io.Fonts.GlyphRangesDefault;

            io.Fonts.AddFontFromFileTTF(fontConfig.Value.Path, size, null, ref *ranges);
        }
        else
        {
            io.Fonts.AddFontDefault(null);
        }

        var pp = new IntPtr();
        var pw = default(int);
        var ph = default(int);
        var ps = default(int);

        var pointer = Unsafe.AsPointer(ref pp);

        io.Fonts.GetTexDataAsRGBA32((byte**)pointer, ref pw, ref ph, ref ps);

        GL.CreateTextures(TextureTarget.Texture2D, 1, out Texture);
        const string labelTEX = "ImGui Texture";
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

        io.Fonts.TexID = (IntPtr)Texture;
        // io.Fonts.SetTexID((IntPtr)Texture); // BUG TODO hide

        io.Fonts.ClearTexData();
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
        //GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
        GL.ObjectLabel(ObjectLabelIdentifier.Buffer, VertexBuffer, labelVBO.Length, labelVBO);
        GL.NamedBufferData(VertexBuffer, VertexBufferSize * sizeOfVBO, IntPtr.Zero, BufferUsageHint.StreamDraw);

        GL.CreateBuffers(1, out IndexBuffer);
        //GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer);
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
        OnWindowKeyPress(e, true);
    }

    private void OnWindowKeyPressUp(KeyboardKeyEventArgs e)
    {
        OnWindowKeyPress(e, false);
    }

    private void OnWindowMouseEnter()
    {
        var position = Window.MouseState.Position;

        IO.AddMousePosEvent(position.X, position.Y);
    }

    private void OnWindowMouseMove(MouseMoveEventArgs e)
    {
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
        OnWindowMouseButton(e);
    }

    private void OnWindowMouseButtonUp(MouseButtonEventArgs e)
    {
        OnWindowMouseButton(e);
    }

    private void OnWindowMouseWheel(MouseWheelEventArgs e)
    {
        IO.AddMouseWheelEvent(e.OffsetX, e.OffsetY);
    }

    private void OnWindowTextInput(TextInputEventArgs e)
    {
        IO.AddInputCharacter((uint)e.Unicode);
    }

    public void Update(float deltaTime)
    {
        using (new ImGuiContextScope(Context))
        {
            //ImPlot.SetCurrentContext(ContextPlot); // TODO
            //ImPlot.SetImGuiContext(Context);

            using (var io = ImGui.GetIO())
            {
                var size = Window.ClientSize;

                io.DisplaySize = new Vector2(size.X, size.Y);

                io.DeltaTime = deltaTime;
            }

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
        if (OperatingSystem.IsWindows())
        {
            UpdateMouseCursorWindows();
        }
    }

    [SupportedOSPlatform("windows")]
    private void UpdateMouseCursorWindows()
    {
#if GLFW_MOUSE_CURSOR_WINDOWS
        // we have to forcibly update cursor else it will only appear briefly because of GLFW

        // until this is properly supported, it's the best since it supports animated cursors

        var bounds = Window.Bounds;

        if (!bounds.ContainsExclusive((Vector2i)(bounds.Min + Window.MouseState.Position)))
        {
            return; // but let system update cursor when mouse is outside, i.e. when resizing
        }

        var io = IO;

        if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) != ImGuiConfigFlags.None)
        {
            return;
        }

        var cursor = ImGuiNET.ImGui.GetMouseCursor();

        if (io.MouseDrawCursor)
        {
            cursor = ImGuiMouseCursor.None;
        }

        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault

        var resource = cursor switch
        {
            ImGuiMouseCursor.None       => ResourceId.NULL,
            ImGuiMouseCursor.Arrow      => User32.IDC_ARROW,
            ImGuiMouseCursor.TextInput  => User32.IDC_IBEAM,
            ImGuiMouseCursor.ResizeAll  => User32.IDC_SIZEALL,
            ImGuiMouseCursor.ResizeNS   => User32.IDC_SIZENS,
            ImGuiMouseCursor.ResizeEW   => User32.IDC_SIZEWE,
            ImGuiMouseCursor.ResizeNESW => User32.IDC_SIZENESW,
            ImGuiMouseCursor.ResizeNWSE => User32.IDC_SIZENWSE,
            ImGuiMouseCursor.Hand       => User32.IDC_HAND,
            ImGuiMouseCursor.NotAllowed => User32.IDC_NO,
            _                           => throw new InvalidEnumArgumentException(null, (int)cursor, typeof(ImGuiMouseCursor))
        };

        User32.SetCursor(User32.LoadCursor(HINSTANCE.NULL, resource));
#endif
    }

    public void Render()
    {
        if (!NewFrame)
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

        using var io = ImGui.GetIO();

        var projection = Matrix4.CreateOrthographicOffCenter(0.0f, io.DisplaySize.X, io.DisplaySize.Y, 0.0f, -1.0f, 1.0f);

        GL.UseProgram(Shader);
        GL.Uniform1(ShaderTexture, 0);
        GL.UniformMatrix4(ShaderProjection, false, ref projection);
        GL.BindSampler(0, 0);
        GL.BindVertexArray(VertexArray);

        var clipOff = data.DisplayPos;
        var clipScale = data.FramebufferScale;

        var offsets = GetOffsets<ImDrawVert>();

        for (var i = 0; i < data.CmdListsCount; i++)
        {
            var list = data.CmdLists[i];

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

            var cmdBuffer = list.CmdBuffer;

            for (var j = 0; j < cmdBuffer.Size; j++)
            {
                var cmd = cmdBuffer[j];

                var clipMin = new Vector2((cmd.ClipRect.X - clipOff.X) * clipScale.X, (cmd.ClipRect.Y - clipOff.Y) * clipScale.Y);
                var clipMax = new Vector2((cmd.ClipRect.Z - clipOff.X) * clipScale.X, (cmd.ClipRect.W - clipOff.Y) * clipScale.Y);

                if (clipMax.X <= clipMin.X || clipMax.Y <= clipMin.Y)
                {
                    continue;
                }

                if (cmd.UserCallback != null)
                {
                    throw new NotImplementedException(); // BUG System.ExecutionEngineException if above continue statement // TODO see header
                }

                GL.Scissor((int)clipMin.X, (int)(scaleY - clipMax.Y), (int)(clipMax.X - clipMin.X), (int)(clipMax.Y - clipMin.Y));

                var texId = cmd.GetTexID();

                GL.BindTextureUnit(0, (int)texId);

                var length = (int)cmd.ElemCount;
                var offset = cmd.IdxOffset * sizeof(ushort);

                if (io.BackendFlags.HasFlags(ImGuiBackendFlags.RendererHasVtxOffset))
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

    private static string GetOffsets<T>()
    {
        var type = typeof(T);

        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        var dictionary = new SortedDictionary<IntPtr, string>();

        foreach (var field in fields)
        {
            var fieldName = field.Name;
            var offsetOf = Marshal.OffsetOf<T>(fieldName);
            dictionary.Add(offsetOf, fieldName);
        }

        var join = string.Join(Environment.NewLine, dictionary);

        return join;
    }
}