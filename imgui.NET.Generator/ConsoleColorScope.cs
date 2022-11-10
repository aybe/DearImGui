namespace imgui.NET.Generator;

public readonly struct ConsoleColorScope : IDisposable
{
    private readonly ConsoleColor BackgroundColor;
    private readonly ConsoleColor ForegroundColor;

    public ConsoleColorScope(ConsoleColor? backgroundColor = null, ConsoleColor? foregroundColor = null)
    {
        BackgroundColor = Console.BackgroundColor;
        ForegroundColor = Console.ForegroundColor;

        if (backgroundColor.HasValue)
            Console.BackgroundColor = backgroundColor.Value;

        if (foregroundColor.HasValue)
            Console.ForegroundColor = foregroundColor.Value;
    }

    public void Dispose()
    {
        Console.BackgroundColor = BackgroundColor;
        Console.ForegroundColor = ForegroundColor;
    }
}