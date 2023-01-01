namespace im.NET.Generator;

public sealed class FileHistory : IDisposable, IAsyncDisposable
{
    private FileHistory(FileInfo info, byte[] data)
    {
        Info = info;
        Data = data;
    }

    public FileInfo Info { get; }

    private byte[] Data { get; }

    private bool IsDisposed { get; set; }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore(false).ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    ~FileHistory()
    {
        Dispose(false);
    }

    private async ValueTask DisposeAsyncCore(bool disposing)
    {
        if (IsDisposed)
            return;

        if (disposing)
        {
            // NOP
        }

        if (Data.Length is not 0)
        {
            Info.Refresh();

            if (Info.Exists)
            {
                var name = Info.FullName;

                var data = await File.ReadAllBytesAsync(name);

                if (data.AsSpan().SequenceEqual(Data) is false)
                {
                    var type = Path.GetExtension(name);

                    name = name[..^type.Length];

                    var path = $"{name}-backup{type}";

                    if (File.Exists(path))
                    {
                        var time = File.GetLastWriteTimeUtc(path);
                        var dest = $"{name}-backup-{time.ToString("O").Replace(':', '-').Replace('.', '-')}{type}";
                        File.Move(path, dest);
                    }

                    await File.WriteAllBytesAsync(path, Data);
                }
            }
        }

        IsDisposed = true;
    }

    private void Dispose(bool disposing)
    {
        DisposeAsyncCore(disposing).AsTask().Wait();
    }

    public static async Task<FileHistory?> CreateAsync(string path, CancellationToken cancellationToken = default)
    {
        var info = new FileInfo(path);

        byte[] data;

        if (info.Exists)
        {
            await using var stream = info.OpenRead();

            data = new byte[stream.Length];

            // ReSharper disable once UnusedVariable

            var read = await stream.ReadAsync(data, cancellationToken);
        }
        else
        {
            data = Array.Empty<byte>();
        }

        return cancellationToken.IsCancellationRequested ? null : new FileHistory(info, data);
    }
}