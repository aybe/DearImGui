namespace ImGuiNET;

partial class ImDrawData
{
    /// <summary> Array of ImDrawList* to render.<br />The ImDrawList are owned by ImGuiContext and only pointed to from here. </summary>
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
}