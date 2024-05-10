using FilesHashApi.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IFileHashProviderService, CachedFileHashProviderService>();

WebApplication app = builder.Build();

app.MapControllers();

app.Run();