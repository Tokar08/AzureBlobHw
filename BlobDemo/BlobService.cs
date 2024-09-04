using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BlobDemo;

internal class BlobService
{
    private readonly string _connectionString;

    public BlobService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<BlobContainerClient> GetContainer(string name)
    {
        BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
        BlobContainerClient container = blobServiceClient.GetBlobContainerClient(name);
        await container.CreateIfNotExistsAsync();
        return container;
    }

    public async Task AddBlob(BlobContainerClient container, string path)
    {
        var name = Path.GetFileName(path);
        BlobClient blobClient = container.GetBlobClient(name);

        if(!File.Exists(path))
        {
            throw new FileNotFoundException("file not found");
        }

        await blobClient.UploadAsync(path);
        Console.WriteLine($"Blob '{name}' was uploaded to Azure");
    }

    public async Task DisplayBlobs(BlobContainerClient container)
    {
        Console.WriteLine("Name\t\tLast Modified\tAccess tier\tSize");
        await foreach(var blob in container.GetBlobsAsync())
        {
            double size = blob.Properties.ContentLength!.Value / 1024.0;
            Console.WriteLine($"{blob.Name}\t\t{blob.Properties.LastModified!.Value.DateTime.ToShortTimeString()}\t\t{blob.Properties.AccessTier}\t\t{size.ToString("F2")} KiB");
        }
    }

    public async Task SetBlobAccessTier(BlobContainerClient container, string name, AccessTier accessTier)
    {
        BlobClient blobClient = container.GetBlobClient(name);
        await blobClient.SetAccessTierAsync(accessTier);
    }

    public async Task SetBlobMetadata(BlobContainerClient container, string name, IDictionary<string, string> metadata)
    {
        BlobClient blobClient = container.GetBlobClient(name);
        await blobClient.SetMetadataAsync(metadata, null, default);
    }

    public async Task DeleteBlob(BlobContainerClient container, string name)
    {
        BlobClient blobClient = container.GetBlobClient(name);
        await blobClient.DeleteIfExistsAsync();
    }
}
