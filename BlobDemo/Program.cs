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

/*
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
*/

var categories = new List<Category>
{
    new Category { Id = 1, Name = "Computers" },
    new Category { Id = 2, Name = "Accessories" },
    new Category { Id = 3, Name = "Monitors" }
};

var products = new List<Product>
{
    new Product { Id = 1, Name = "Gaming Laptop", Price = 999.99f, Category = categories.First(c => c.Id == 1) },
    new Product { Id = 2, Name = "Business PC", Price = 899.99f, Category = categories.First(c => c.Id == 1) },
    new Product { Id = 3, Name = "Mechanical Keyboard", Price = 799.99f, Category = categories.First(c => c.Id == 2) },
    new Product { Id = 4, Name = "Wireless Mouse", Price = 699.99f, Category = categories.First(c => c.Id == 2) },
    new Product { Id = 5, Name = "4K Monitor", Price = 599.99f, Category = categories.First(c => c.Id == 3) },
    new Product { Id = 6, Name = "Gaming Monitor", Price = 499.99f, Category = categories.First(c => c.Id == 3) }
};


await blobService.AddObjectsAsync(categories, "categories.json", container);
await blobService.AddObjectsAsync(products, "products.json", container);

Console.WriteLine(new string('-', 40));

var downloadedCategories = await blobService.DownloadObjectsAsync<Category>("categories.json", container);

foreach (var category in downloadedCategories)
{
    Console.WriteLine($"Downloaded Category: {category.Name}");
}

Console.WriteLine(new string('-', 40));

var downloadedProducts = await blobService.DownloadObjectsAsync<Product>("products.json", container);
foreach (var product in downloadedProducts)
{
    Console.WriteLine($"Downloaded Product: {product.Name}, Price: {product.Price}, Category: {product.Category.Name}");
}