namespace ImGuiNET.OpenTK;

public readonly struct ImGuiFont
{
    public string Path { get; }

    public float Size { get; }

    public ImGuiFont(string path, float size)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(null, path);
        }

        if (size <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        Path = path;
        Size = size;
    }

    public override string ToString()
    {
        return $"{nameof(Path)}: {Path}, {nameof(Size)}: {Size}";
    }
}