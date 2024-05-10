using FilesHashApi;
using FilesHashApi.Services;
using Microsoft.AspNetCore.Mvc;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddSingleton<IFileHashProviderService, CachedFileHashProviderService>();

WebApplication app = builder.Build();

app.Map("/", () => "FilesHashApi is running!");
app.MapGet("/api/hashes/md5", async ([FromQuery] string file, IFileHashProviderService hashProviderService) =>
{
    string hash = await hashProviderService.GetMd5HashAsync(file);
    return Results.Text(hash, statusCode: 200);
});

app.Run();
