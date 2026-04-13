namespace VypusknykPlus.Application.Services;

public interface IImageService
{
    /// <summary>
    /// Upload an image stream for a product. Returns the stored object key (e.g. "products/42.jpg").
    /// </summary>
    Task<string> UploadProductImageAsync(int productId, Stream imageStream, string contentType, CancellationToken ct = default);

    /// <summary>
    /// Delete an object by its key. Silently no-ops if the object does not exist.
    /// </summary>
    Task DeleteAsync(string objectKey, CancellationToken ct = default);

    /// <summary>
    /// Build the public browser-accessible URL from a stored object key.
    /// Returns null when imageKey is null or empty.
    /// </summary>
    string? GetPublicUrl(string? imageKey);

    /// <summary>
    /// Ensure the configured bucket exists and has a public read policy applied.
    /// Called once at application startup.
    /// </summary>
    Task EnsureBucketAsync(CancellationToken ct = default);
}
