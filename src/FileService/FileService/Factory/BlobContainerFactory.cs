using Azure.Storage.Blobs;

namespace FileService.Factory;

public class BlobContainerFactory
{
    private IConfiguration Configuration { get; }
    private IWebHostEnvironment Environment { get; }

    public BlobContainerFactory(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public BlobContainerClient CreateClient()
    {
        var connectionString = Configuration.GetConnectionString("StorageAccount");
        var environment = Environment.EnvironmentName.ToLower();

        return new BlobContainerClient(connectionString, environment);
    }
}
