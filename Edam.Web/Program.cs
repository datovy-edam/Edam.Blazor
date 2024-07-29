using Edam.Web;
using Edam.Web.Components;
using KristofferStrube.Blazor.FileSystem;
using KristofferStrube.Blazor.FileSystemAccess;
using KristofferStrube.Blazor.FileAPI;
using MudBlazor.Services;
using Edam.Web.FileSystemHelper;
using Edam.Web.Models.Application;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.AddRedisOutputCache("redis");
builder.AddKeyedSqlServerClient("edamdb");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
   // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
   // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
   client.BaseAddress = new("https+http://apiservice");
});

builder.Services.AddHttpClient<FileSystemApiClient>(client =>
{
   // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
   // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
   client.BaseAddress = new("https+http://apifilesystemservice");
});

builder.Services.AddMudServices();

builder.Services.AddURLService();

builder.Services.AddFileSystemAccessService();
builder.Services.AddStorageManagerService();

builder.Services.AddScoped<AppSession>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
   app.UseExceptionHandler("/Error", createScopeForErrors: true);
   // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();

