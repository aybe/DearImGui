using System.Text;

namespace ImGuiNET.Generator;

public sealed class AggregateTextWriter : TextWriter
{
    private readonly TextWriter Writer1;

    private readonly TextWriter[] Writers;

    public AggregateTextWriter(params TextWriter[] writers)
    {
        Writer1 = writers.First();
        Writers = writers;
    }

    public AggregateTextWriter(IEnumerable<TextWriter> writers)
        : this(writers as TextWriter[] ?? writers.ToArray())
    {
    }

    public override Encoding Encoding => Writer1.Encoding;

    #region Sync

    public override void Close()
    {
        ForEach(s => s.Close());
    }

    public override void Flush()
    {
        ForEach(s => s.Flush());
    }

    public override void Write(bool value)
    {
        ForEach(s => s.Write(value));
    }

    public override void Write(char value)
    {
        ForEach(s => s.Write(value));
    }

    public override void Write(char[]? buffer)
    {
        ForEach(s => s.Write(buffer));
    }

    public override void Write(char[] buffer, int index, int count)
    {
        ForEach(s => s.Write(buffer, index, count));
    }

    public override void Write(decimal value)
    {
        ForEach(s => s.Write(value));
    }

    public override void Write(double value)
    {
        ForEach(s => s.Write(value));
    }

    public override void Write(int value)
    {
        ForEach(s => s.Write(value));
    }

    public override void Write(long value)
    {
        ForEach(s => s.Write(value));
    }

    public override void Write(object? value)
    {
        ForEach(s => s.Write(value));
    }

    public override void Write(ReadOnlySpan<char> buffer)
    {
        foreach (var writer in Writers)
        {
            writer.WriteLine(buffer);
        }
    }

    public override void Write(float value)
    {
        ForEach(s => s.Write(value));
    }

    public override void Write(string? value)
    {
        ForEach(s => s.Write(value));
    }

    public override void Write(string format, object? arg0)
    {
        ForEach(s => s.Write(format, arg0));
    }

    public override void Write(string format, object? arg0, object? arg1)
    {
        ForEach(s => s.Write(format, arg0, arg1));
    }

    public override void Write(string format, object? arg0, object? arg1, object? arg2)
    {
        ForEach(s => s.Write(format, arg0, arg1, arg2));
    }

    public override void Write(string format, params object?[] arg)
    {
        ForEach(s => s.Write(format, arg));
    }

    public override void Write(StringBuilder? value)
    {
        ForEach(s => s.Write(value));
    }

    public override void Write(uint value)
    {
        ForEach(s => s.Write(value));
    }

    public override void Write(ulong value)
    {
        ForEach(s => s.Write(value));
    }

    public override void WriteLine()
    {
        ForEach(s => s.WriteLine());
    }

    public override void WriteLine(bool value)
    {
        ForEach(s => s.WriteLine(value));
    }

    public override void WriteLine(char value)
    {
        ForEach(s => s.WriteLine(value));
    }

    public override void WriteLine(char[]? buffer)
    {
        ForEach(s => s.WriteLine(buffer));
    }

    public override void WriteLine(char[] buffer, int index, int count)
    {
        ForEach(s => s.WriteLine(buffer, index, count));
    }

    public override void WriteLine(decimal value)
    {
        ForEach(s => s.WriteLine(value));
    }

    public override void WriteLine(double value)
    {
        ForEach(s => s.WriteLine(value));
    }

    public override void WriteLine(int value)
    {
        ForEach(s => s.WriteLine(value));
    }

    public override void WriteLine(long value)
    {
        ForEach(s => s.WriteLine(value));
    }

    public override void WriteLine(object? value)
    {
        ForEach(s => s.WriteLine(value));
    }

    public override void WriteLine(ReadOnlySpan<char> buffer)
    {
        foreach (var writer in Writers)
        {
            writer.WriteLine(buffer);
        }
    }

    public override void WriteLine(float value)
    {
        ForEach(s => s.WriteLine(value));
    }

    public override void WriteLine(string? value)
    {
        ForEach(s => s.WriteLine(value));
    }

    public override void WriteLine(string format, object? arg0)
    {
        ForEach(s => s.WriteLine(format, arg0));
    }

    public override void WriteLine(string format, object? arg0, object? arg1)
    {
        ForEach(s => s.WriteLine(format, arg0, arg1));
    }

    public override void WriteLine(string format, object? arg0, object? arg1, object? arg2)
    {
        ForEach(s => s.WriteLine(format, arg0, arg1, arg2));
    }

    public override void WriteLine(string format, params object?[] arg)
    {
        ForEach(s => s.WriteLine(format, arg));
    }

    public override void WriteLine(StringBuilder? value)
    {
        ForEach(s => s.WriteLine(value));
    }

    public override void WriteLine(uint value)
    {
        ForEach(s => s.WriteLine(value));
    }

    public override void WriteLine(ulong value)
    {
        ForEach(s => s.WriteLine(value));
    }

    #endregion

    #region Async

    public override ValueTask DisposeAsync()
    {
        return WhenAll(s => s.DisposeAsync());
    }

    public override Task FlushAsync()
    {
        return WhenAll(s => s.FlushAsync());
    }

    public override Task WriteAsync(char value)
    {
        return WhenAll(s => s.WriteAsync(value));
    }

    public override Task WriteAsync(char[] buffer, int index, int count)
    {
        return WhenAll(s => s.WriteAsync(buffer, index, count));
    }

    public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = new())
    {
        return WhenAll(s => s.WriteAsync(buffer, cancellationToken));
    }

    public override Task WriteAsync(string? value)
    {
        return WhenAll(s => s.WriteAsync(value));
    }

    public override Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = new())
    {
        return WhenAll(s => s.WriteAsync(value, cancellationToken));
    }

    public override Task WriteLineAsync()
    {
        return WhenAll(s => s.WriteLineAsync());
    }

    public override Task WriteLineAsync(char value)
    {
        return WhenAll(s => s.WriteLineAsync(value));
    }

    public override Task WriteLineAsync(char[] buffer, int index, int count)
    {
        return WhenAll(s => s.WriteLineAsync(buffer, index, count));
    }

    public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = new())
    {
        return WhenAll(s => s.WriteLineAsync(buffer, cancellationToken));
    }

    public override Task WriteLineAsync(string? value)
    {
        return WhenAll(s => s.WriteLineAsync(value));
    }

    public override Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = new())
    {
        return WhenAll(s => s.WriteLineAsync(value, cancellationToken));
    }

    #endregion

    #region Helpers

    private void ForEach(Action<TextWriter> action)
    {
        Array.ForEach(Writers, action);
    }

    private Task WhenAll(Func<TextWriter, Task> func)
    {
        return Task.WhenAll(Writers.Select(func));
    }

    private ValueTask WhenAll(Func<TextWriter, ValueTask> func)
    {
        return new ValueTask(Task.WhenAll(Writers.Select(s => func(s).AsTask())));
    }

    #endregion
}