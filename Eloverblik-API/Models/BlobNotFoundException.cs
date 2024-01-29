namespace Eloverblik_API.Models;

public class BlobNotFoundException : Exception
{
    public BlobNotFoundException(string blobName, string containerName)
        : base($"Blob '{blobName}' does not exist in container '{containerName}'")
    {
    }
}