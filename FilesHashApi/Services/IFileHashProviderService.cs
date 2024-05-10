namespace FilesHashApi.Services;

public interface IFileHashProviderService
{
    public Task<string> GetMd5HashAsync(string file);
}