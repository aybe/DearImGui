namespace imgui.NET.OpenTK;

public readonly struct ImGuiFontConfig
{
    public string Path { get; }

    public float Size { get; }

    public ImGuiFontConfig(string path, float size)
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