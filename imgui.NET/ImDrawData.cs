using JetBrains.Annotations;

namespace imgui.NET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImDrawData
{
    public unsafe ImDrawList[] CmdLists
    {
        get
        {
            var lists = new ImDrawList[CmdListsCount];

            for (var i = 0; i < lists.Length; i++)
            {
                lists[i] = ImDrawList.__GetOrCreateInstance(((IntPtr*)((__Internal*)__Instance)->CmdLists)[i]);
            }

            return lists;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(Valid)}: {Valid}, {nameof(TotalIdxCount)}: {TotalIdxCount}, {nameof(TotalVtxCount)}: {TotalVtxCount}, {nameof(CmdListsCount)}: {CmdListsCount}";
    }
}