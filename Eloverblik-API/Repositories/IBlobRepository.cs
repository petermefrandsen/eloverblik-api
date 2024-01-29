namespace Eloverblik_API.Repositories;

public interface IBlobRepository
{
    public Task<List<string[]>> GetBlobContentAsync(string blobFtpContainerName, string blobName);
    public Task UploadBlobToPath(dynamic content, string path, string blobName);
    Task<bool> TestBlobStorageConnection();
}
