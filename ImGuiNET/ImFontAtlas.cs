using System.Numerics;
using JetBrains.Annotations;

namespace ImGuiNET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImFontAtlas
{
    public unsafe Vector4[] TexUvLines
    {
        get
        {
            var value = new Vector4[64];

            Marshalling.Copy(ref value, ((__Internal*)__Instance)->TexUvLines);

            return value;
        }

        set
        {
            ArgumentNullException.ThrowIfNull(value);

            Marshalling.Copy(((__Internal*)__Instance)->TexUvLines, ref value);
        }
    }
}