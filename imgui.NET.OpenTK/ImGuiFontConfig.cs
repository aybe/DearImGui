#pragma warning disable CS1591
namespace imgui.NET.OpenTK;

/// <summary>
///     Specifies a font to use by <see cref="ImGuiController" />.
/// </summary>
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

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(Path)}: {Path}, {nameof(Size)}: {Size}";
    }
}