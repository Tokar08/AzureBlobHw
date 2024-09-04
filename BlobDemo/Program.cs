using BlobDemo;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
            .AddJsonFile("config.json")
            .Build();

string connectionString = config.GetConnectionString("Default") ?? throw new NullReferenceException("Connection string not found");

var blobService = new BlobService(connectionString);

var container = await blobService.GetContainer("temp");
string path = "";
//await blobService.AddBlob(container, Path.Combine(path, "fox.jpg"));
//await blobService.AddBlob(container, Path.Combine(path, "bear.jpg"));
//await blobService.SetBlobAccessTier(container, "bear.jpg", AccessTier.Cold);
/*await blobService.SetBlobMetadata(container, "bear.jpg", new Dictionary<string, string>()
    { 
        { "Name", "Bear" }, 
        { "Location", "Forest" }, 
    }
);*/
//await blobService.DeleteBlob(container, "bear.jpg");
/*var sas = await blobService.GetSAS(container, "fox.jpg");
Console.WriteLine(sas);*/
//await blobService.DownloadBlob(container, "fox.jpg");


for(int i = 0; i < 5; i++)
{
    File.Copy(Path.Combine(path, "bear.jpg"), Path.Combine(path, $"bear{i}.jpg"));
    await blobService.AddBlob(container, Path.Combine(path, $"bear{i}.jpg"));
}

await blobService.DisplayBlobs(container);

await blobService.DeleteMultipleBlobs(container, 
    Enumerable.Range(0, 5).Select(i => $"bear{i}.jpg"));

Console.WriteLine(new string('-', 40));
await blobService.DisplayBlobs(container);