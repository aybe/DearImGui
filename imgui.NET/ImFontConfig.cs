namespace imgui.NET;

unsafe partial class ImFontConfig
{
    public string Name => Marshalling.String(((__Internal*)__Instance)->Name);
}