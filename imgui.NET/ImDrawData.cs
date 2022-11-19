using JetBrains.Annotations;

namespace imgui.NET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImDrawData
{
    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2639
    /// </summary>
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