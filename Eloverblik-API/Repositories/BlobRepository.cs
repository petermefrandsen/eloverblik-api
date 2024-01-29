using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Eloverblik_API.Models;
using System.Text;
using System.Text.Json;

namespace Eloverblik_API.Repositories;

public class BlobRepository : IBlobRepository
{
    private readonly string _blobConnectionString;

    public BlobRepository()
    {
        _blobConnectionString = Environment.GetEnvironmentVariable("blobStorageConnectionString") ?? "";
    }

    public async Task<List<string[]>> GetBlobContentAsync(string blobFtpContainerName, string blobName)
    {
        if(string.IsNullOrEmpty(_blobConnectionString))
        {
            throw new MissingConnectionStringException("Connection string for blob storage FTP is not set");
        }

        try
        {
            var blobServiceClientFTP = new BlobServiceClient(_blobConnectionString);
            var containerFTP = blobServiceClientFTP.GetBlobContainerClient(blobFtpContainerName);

            if (!containerFTP.GetBlockBlobClient(blobName).Exists())
            {
                throw new BlobNotFoundException(blobName, blobFtpContainerName);
            }

            BlobClient blobClient = containerFTP.GetBlobClient(blobName);

            var blob = await blobClient.DownloadAsync();
            return await GetColumnsFromBlob(blob);
        }
        catch
        {
            throw new Exception($"Error at retrieving blob: {blobName}");
        }
    }

    public async Task UploadBlobToPath(dynamic content, string path, string blobName)
    {
        if (string.IsNullOrEmpty(_blobConnectionString))
        {
            throw new MissingConnectionStringException("Connection string for blob storage for processed files is not set");
        }

        try
        {
            var blobContent = CreateFileStream(content);
            var blobnew = new BlobClient(_blobConnectionString, path, blobName);
            blobContent.Position = 0;
            await blobnew.UploadAsync(blobContent);
        }
        catch
        {
            throw new Exception($"Error at uploading blob: {blobName}, to destination: {path}");
        }
    }

    private static Stream CreateFileStream(dynamic content)
    {
        var blobContent = JsonSerializer.Serialize(content);
        var blobByte = Encoding.UTF8.GetBytes(blobContent);
        var blobStream = new MemoryStream();
        blobStream.Write(blobByte, 0, blobByte.Length);
        return blobStream;
    }

    private static async Task<List<string[]>> GetColumnsFromBlob(Response<BlobDownloadInfo> blob)
    {
        var blobContent = new List<string[]>();

        using (var streamReader = new StreamReader(blob.Value.Content))
        {
            while (!streamReader.EndOfStream)
            {
                var line = await streamReader.ReadLineAsync();

                if (line == null)
                {
                    continue;
                }

                blobContent.Add(line.Split(';'));
            }
        }

        return blobContent;
    }

    public async Task<bool> TestBlobStorageConnection()
    {
        try
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_blobConnectionString);
            await blobServiceClient.GetPropertiesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect to blob storage: {ex.Message}");
            return false;
        }
    }
}
