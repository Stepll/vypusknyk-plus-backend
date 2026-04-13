namespace VypusknykPlus.Application.Services;

public class MinioSettings
{
    public string Endpoint { get; set; } = string.Empty;        // internal: "minio:9000" in Docker
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = "products";
    public string PublicEndpoint { get; set; } = string.Empty;  // browser-accessible: "http://localhost:9000"
    public bool UseSSL { get; set; } = false;
}
