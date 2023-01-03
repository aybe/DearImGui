#pragma warning disable CS1591
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace DearImGui.OpenTK.Extensions;

/// <summary>
///     Scope for temporarily changing OpenGL state.
/// </summary>
public readonly struct OpenGLStateScope : IDisposable
{
    private readonly TextureUnit ActiveTexture;
    private readonly int ArrayBufferBinding;
    private readonly bool Blend;
    private readonly BlendingFactorDest BlendDstAlpha;
    private readonly BlendingFactorDest BlendDstRgb;
    private readonly BlendEquationMode BlendEquationAlpha;
    private readonly BlendEquationMode BlendEquationRgb;
    private readonly BlendingFactorSrc BlendSrcAlpha;
    private readonly BlendingFactorSrc BlendSrcRgb;
    private readonly bool CullFace;
    private readonly int CurrentProgram;
    private readonly bool DepthTest;
    private readonly int PolygonMode;
    private readonly bool PrimitiveRestart;
    private readonly int SamplerBinding;
    private readonly Vector4i ScissorBox;
    private readonly bool ScissorTest;
    private readonly bool StencilTest;
    private readonly int TextureBinding2D;
    private readonly int VertexArrayBinding;
    private readonly Vector4i Viewport;

    public OpenGLStateScope()
    {
        ActiveTexture      = (TextureUnit)GL.GetInteger(GetPName.ActiveTexture);
        ArrayBufferBinding = GL.GetInteger(GetPName.ArrayBufferBinding);
        Blend              = GL.GetBoolean(GetPName.Blend);
        BlendDstAlpha      = (BlendingFactorDest)GL.GetInteger(GetPName.BlendDstAlpha);
        BlendDstRgb        = (BlendingFactorDest)GL.GetInteger(GetPName.BlendDstRgb);
        BlendEquationAlpha = (BlendEquationMode)GL.GetInteger(GetPName.BlendEquationAlpha);
        BlendEquationRgb   = (BlendEquationMode)GL.GetInteger(GetPName.BlendEquationRgb);
        BlendSrcAlpha      = (BlendingFactorSrc)GL.GetInteger(GetPName.BlendSrcAlpha);
        BlendSrcRgb        = (BlendingFactorSrc)GL.GetInteger(GetPName.BlendSrcRgb);
        CullFace           = GL.GetBoolean(GetPName.CullFace);
        CurrentProgram     = GL.GetInteger(GetPName.CurrentProgram);
        DepthTest          = GL.GetBoolean(GetPName.DepthTest);
        PolygonMode        = GL.GetInteger(GetPName.PolygonMode);
        PrimitiveRestart   = GL.IsEnabled(EnableCap.PrimitiveRestart);
        SamplerBinding     = GL.GetInteger(GetPName.SamplerBinding);
        ScissorBox         = GetVector4i(GetPName.ScissorBox);
        ScissorTest        = GL.GetBoolean(GetPName.ScissorTest);
        StencilTest        = GL.GetBoolean(GetPName.StencilTest);
        TextureBinding2D   = GL.GetInteger(GetPName.TextureBinding2D);
        VertexArrayBinding = GL.GetInteger(GetPName.VertexArrayBinding);
        Viewport           = GetVector4i(GetPName.Viewport);
    }

    #region IDisposable Members

    /// <inheritdoc />
    public void Dispose()
    {
        GL.ActiveTexture(ActiveTexture);
        GL.BindBuffer(BufferTarget.ArrayBuffer, ArrayBufferBinding);
        GL.BindSampler(0, SamplerBinding);
        GL.BindTexture(TextureTarget.Texture2D, TextureBinding2D);
        GL.BindVertexArray(VertexArrayBinding);
        GL.BlendEquationSeparate(BlendEquationRgb, BlendEquationAlpha);
        GL.BlendFuncSeparate(BlendSrcRgb, BlendDstRgb, BlendSrcAlpha, BlendDstAlpha);
        GL.PolygonMode(MaterialFace.FrontAndBack, (PolygonMode)PolygonMode);
        GL.Scissor(ScissorBox.X, ScissorBox.Y, ScissorBox.Z, ScissorBox.W);
        GL.UseProgram(CurrentProgram);
        GL.Viewport(Viewport.X, Viewport.Y, Viewport.Z, Viewport.W);

        SetEnableCap(EnableCap.Blend, Blend);
        SetEnableCap(EnableCap.CullFace, CullFace);
        SetEnableCap(EnableCap.DepthTest, DepthTest);
        SetEnableCap(EnableCap.PrimitiveRestart, PrimitiveRestart);
        SetEnableCap(EnableCap.ScissorTest, ScissorTest);
        SetEnableCap(EnableCap.StencilTest, StencilTest);
    }

    #endregion

    private static Vector4i GetVector4i(GetPName name)
    {
        unsafe
        {
            var result = new Vector4i();

            GL.GetInteger(name, &result.X);

            return result;
        }
    }

    private static void SetEnableCap(EnableCap enableCap, bool enabled)
    {
        if (enabled)
        {
            GL.Enable(enableCap);
        }
        else
        {
            GL.Disable(enableCap);
        }
    }
}