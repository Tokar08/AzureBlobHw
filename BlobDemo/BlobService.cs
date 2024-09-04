using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

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

    public async Task AddSnapshot(BlobContainerClient container, string name)
    {
        BlobClient blobClient = container.GetBlobClient(name);
        await blobClient.CreateSnapshotAsync();
    }

    public async Task<string> GetSAS(BlobContainerClient container, string name)
    {
        BlobClient blobClient = container.GetBlobClient(name);

        if (!blobClient.CanGenerateSasUri)
        {
            throw new ArgumentException("blob cannot generate sas");
        }
        BlobSasBuilder builder = new BlobSasBuilder()
        {
            BlobContainerName = container.Name,
            BlobName = name,
            Resource = "b",
            ExpiresOn = DateTime.UtcNow.AddMinutes(10),
        };
        builder.SetPermissions(BlobAccountSasPermissions.Read | BlobAccountSasPermissions.Write);
        Uri uri = blobClient.GenerateSasUri(builder);

        return uri.ToString();
    }

    public async Task DownloadBlob(BlobContainerClient container, string name)
    {
        BlobClient blobClient = container.GetBlobClient(name);

        if (!Directory.Exists("data"))
        {
            Directory.CreateDirectory("data");
        }

        await blobClient.DownloadToAsync(Path.Combine("data", name));
    }

    public async Task DeleteMultipleBlobs(BlobContainerClient container, IEnumerable<string> names)
    {
        BlobBatchClient blobBatchClient = container.GetBlobBatchClient();
        int count = names.Count();
        List<Uri> uris = new List<Uri>(capacity: count);
        foreach (string name in names)
        {
            var blob = container.GetBlobClient(name);
            uris.Add(blob.Uri);
        }

        await blobBatchClient.DeleteBlobsAsync(uris);
    }
}
