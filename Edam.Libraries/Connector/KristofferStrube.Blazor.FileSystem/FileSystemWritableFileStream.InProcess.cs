using KristofferStrube.Blazor.FileSystem.Extensions;
using Microsoft.JSInterop;

namespace KristofferStrube.Blazor.FileSystem;

/// <summary>
/// <see href="https://fs.spec.whatwg.org/#filesystemwritablefilestream">FileSystemWritableFileStream browser specs</see>
/// </summary>
public class FileSystemWritableFileStreamInProcess : FileSystemWritableFileStream
{
    public new IJSInProcessObjectReference JSReference;
    protected readonly IJSInProcessObjectReference inProcessHelper;

    public static async Task<FileSystemWritableFileStreamInProcess> CreateAsync(IJSRuntime jSRuntime, IJSInProcessObjectReference jSReference)
    {
        return await CreateAsync(jSRuntime, jSReference, FileSystemOptions.DefaultInstance);
    }

    public static async Task<FileSystemWritableFileStreamInProcess> CreateAsync(IJSRuntime jSRuntime, IJSInProcessObjectReference jSReference, FileSystemOptions options)
    {
        IJSInProcessObjectReference inProcessHelper = await jSRuntime.GetInProcessHelperAsync(options);
        WebIDL.CreationOptions creationOptions = new WebIDL.CreationOptions();
        return new FileSystemWritableFileStreamInProcess(jSRuntime, inProcessHelper, jSReference, creationOptions);
    }

    internal FileSystemWritableFileStreamInProcess(IJSRuntime jSRuntime, IJSInProcessObjectReference inProcessHelper, IJSInProcessObjectReference jSReference, WebIDL.CreationOptions options) : base(jSRuntime, jSReference, options)
    {
        this.inProcessHelper = inProcessHelper;
        JSReference = jSReference;
    }
}
