using JetBrains.Annotations;

namespace im.NET.Generator.Logging;

[PublicAPI]
public sealed class AggregateConsoleOut : IDisposable
{
    private readonly TextWriter Out;

    private readonly AggregateTextWriter Writer;

    public AggregateConsoleOut(params TextWriter[] writers)
    {
        Console.SetOut(Writer = new AggregateTextWriter(writers.Append(Out = Console.Out)));
    }

    #region IDisposable Members

    public void Dispose()
    {
        Console.SetOut(Out);

        Writer.Dispose();
    }

    #endregion
}