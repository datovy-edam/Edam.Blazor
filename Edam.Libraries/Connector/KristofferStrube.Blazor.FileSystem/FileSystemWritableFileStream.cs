using KristofferStrube.Blazor.FileAPI;
using KristofferStrube.Blazor.FileSystem.Extensions;
using KristofferStrube.Blazor.Streams;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace KristofferStrube.Blazor.FileSystem;

/// <summary>
/// <see href="https://fs.spec.whatwg.org/#filesystemwritablefilestream">FileSystemWritableFileStream browser specs</see>
/// </summary>
public class FileSystemWritableFileStream : WritableStream
{
    /// <summary>
    /// A lazily evaluated task that gives access to helper methods for the File System API.
    /// </summary>
    protected readonly Lazy<Task<IJSObjectReference>> fileSystemHelperTask;

    public override bool CanSeek => true;

    public override long Position { get; set; }

    [Obsolete("This will be removed in the next major release as all creator methods should be asynchronous for uniformity. Use CreateAsync instead.")]
    public static new FileSystemWritableFileStream Create(IJSRuntime jSRuntime, IJSObjectReference jSReference, WebIDL.CreationOptions options)
   {
        return new FileSystemWritableFileStream(jSRuntime, jSReference, options);
    }

    public static new Task<FileSystemWritableFileStream> CreateAsync(IJSRuntime jSRuntime, IJSObjectReference jSReference, WebIDL.CreationOptions options)
    {
        return Task.FromResult(new FileSystemWritableFileStream(jSRuntime, jSReference, options));
    }

    protected FileSystemWritableFileStream(IJSRuntime jSRuntime, IJSObjectReference jSReference, WebIDL.CreationOptions options) : base(jSRuntime, jSReference, options)
    {
        fileSystemHelperTask = new(() => jSRuntime.GetHelperAsync(FileSystemOptions.DefaultInstance));
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        await JSReference.InvokeVoidAsync("write", buffer.ToArray());
    }

    public async Task WriteAsync(string data)
    {
        await JSReference.InvokeVoidAsync("write", data);
    }

    public async Task WriteAsync(byte[] data)
    {
        await JSReference.InvokeVoidAsync("write", data);
    }

    public async Task WriteAsync(Blob data)
    {
        await JSReference.InvokeVoidAsync("write", data.JSReference);
    }

    public async Task WriteAsync(BlobWriteParams data)
    {
        await JSReference.InvokeVoidAsync("write", data);
    }

    public async Task WriteAsync(StringWriteParams data)
    {
        await JSReference.InvokeVoidAsync("write", data);
    }

    public async Task WriteAsync(ByteArrayWriteParams data)
    {
        await JSReference.InvokeVoidAsync("write", data);
    }

    public async Task SeekAsync(ulong position)
    {
        Position = (long)position;
        await JSReference.InvokeVoidAsync("seek", position);
    }

    public async Task TruncateAsync(ulong size)
    {
        await JSReference.InvokeVoidAsync("truncate", size);
    }
}
