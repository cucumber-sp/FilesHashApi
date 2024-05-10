using System.Security.Cryptography;

namespace FilesHashApi.Services;

public class CachedFileHashProviderService : IFileHashProviderService
{
    List<CachedFileHash> cachedFileHashes = [];
    List<string> updatingFiles = [];

    const int CacheTime = 60 * 10; // 10 minutes
    
    public async Task<string> GetMd5HashAsync(string file)
    {
        CachedFileHash? cachedFileHash = cachedFileHashes.FirstOrDefault(x => x.File == file);
        if (cachedFileHash is not null && DateTime.Now - cachedFileHash.CreatedAt < TimeSpan.FromSeconds(CacheTime))
            return cachedFileHash.Md5;
        
        if (!updatingFiles.Contains(file))
            await UpdateFile(file);
        while (updatingFiles.Contains(file))
            await Task.Delay(100);
        
        cachedFileHash = cachedFileHashes.FirstOrDefault(x => x.File == file);
        if (cachedFileHash is null)
            throw new Exception("Cannot get file hash.");
        
        return cachedFileHash.Md5;
    }

    async Task UpdateFile(string link)
    {
        try
        {
            updatingFiles.Add(link);
            using HttpClient client = new();
            using HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, link));
            if (response.Content.Headers.ContentMD5 is not null)
            {
                CachedFileHash? existing = cachedFileHashes.FirstOrDefault(x => x.File == link);
                if (existing is not null)
                    cachedFileHashes.Remove(existing);
                cachedFileHashes.Add(new CachedFileHash
                {
                    File = link,
                    Md5 = Convert.ToBase64String(response.Content.Headers.ContentMD5),
                    CreatedAt = DateTime.Now
                });
                return;
            }
            using HttpResponseMessage fileResponse = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, link));
            await using Stream stream = await fileResponse.Content.ReadAsStreamAsync();
            using MD5 md5 = MD5.Create();
            byte[] hash = await md5.ComputeHashAsync(stream);
            string md5Str = Convert.ToBase64String(hash);
            CachedFileHash? existingHash = cachedFileHashes.FirstOrDefault(x => x.File == link);
            if (existingHash is not null)
                cachedFileHashes.Remove(existingHash);
            cachedFileHashes.Add(new CachedFileHash
            {
                File = link,
                Md5 = md5Str,
                CreatedAt = DateTime.Now
            });
            stream.Close();
        }
        finally
        {
            updatingFiles.Remove(link);
        }
    }
    
    class CachedFileHash
    {
        public string File { get; set; }
        public string Md5 { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}