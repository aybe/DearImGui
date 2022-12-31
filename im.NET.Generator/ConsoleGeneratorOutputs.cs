namespace im.NET.Generator;

public sealed class ConsoleGeneratorOutputs
{
    public ConsoleGeneratorOutputs(string csPathX32, string csPathX64, string csPathAny)
    {
        CsPathX32 = csPathX32;
        CsPathX64 = csPathX64;
        CsPathAny = csPathAny;
    }

    public string CsPathX32 { get; }

    public string CsPathX64 { get; }

    public string CsPathAny { get; }
}