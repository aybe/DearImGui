using JetBrains.Annotations;

namespace imgui.NET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
unsafe partial class ImFontConfig
{
    public string Name => Marshalling.ReadString(((__Internal*)__Instance)->Name);
}