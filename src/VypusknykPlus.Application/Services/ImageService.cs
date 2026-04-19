using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace VypusknykPlus.Application.Services;

public class ImageService : IImageService
{
    private readonly IMinioClient _minio;
    private readonly MinioSettings _settings;
    private readonly ILogger<ImageService> _logger;

    public ImageService(IMinioClient minio, IOptions<MinioSettings> settings, ILogger<ImageService> logger)
    {
        _minio = minio;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> UploadAsync(string objectKey, Stream imageStream, string contentType, CancellationToken ct = default)
    {
        var putArgs = new PutObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectKey)
            .WithStreamData(imageStream)
            .WithObjectSize(imageStream.Length)
            .WithContentType(contentType);

        await _minio.PutObjectAsync(putArgs, ct);
        _logger.LogInformation("Image uploaded: {ObjectKey}", objectKey);
        return objectKey;
    }

    public async Task<string> UploadProductImageAsync(
        long productId, Stream imageStream, string contentType, CancellationToken ct = default)
    {
        var extension = contentType switch
        {
            "image/jpeg" => "jpg",
            "image/png"  => "png",
            "image/webp" => "webp",
            _            => "jpg"
        };
        var objectKey = $"products/{productId}.{extension}";
        return await UploadAsync(objectKey, imageStream, contentType, ct);
    }

    public async Task DeleteAsync(string objectKey, CancellationToken ct = default)
    {
        try
        {
            var removeArgs = new RemoveObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(objectKey);

            await _minio.RemoveObjectAsync(removeArgs, ct);
            _logger.LogInformation("Image deleted: {ObjectKey}", objectKey);
        }
        catch (Exception ex)
        {
            // A missing image must not block product operations
            _logger.LogWarning(ex, "Failed to delete image {ObjectKey} from MinIO", objectKey);
        }
    }

    public string? GetPublicUrl(string? imageKey)
    {
        if (string.IsNullOrEmpty(imageKey))
            return null;

        var host = string.IsNullOrEmpty(_settings.PublicEndpoint)
            ? _settings.Endpoint
            : _settings.PublicEndpoint;

        return $"{host.TrimEnd('/')}/{_settings.BucketName}/{imageKey}";
    }

    public async Task EnsureBucketAsync(CancellationToken ct = default)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_settings.BucketName);
        var exists = await _minio.BucketExistsAsync(bucketExistsArgs, ct);

        if (!exists)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(_settings.BucketName);
            await _minio.MakeBucketAsync(makeBucketArgs, ct);
            _logger.LogInformation("MinIO bucket '{Bucket}' created", _settings.BucketName);
        }

        // Public read policy — allows anonymous GET on any object in the bucket
        var policy = $$"""
            {
              "Version": "2012-10-17",
              "Statement": [{
                "Effect": "Allow",
                "Principal": {"AWS": ["*"]},
                "Action": ["s3:GetObject"],
                "Resource": ["arn:aws:s3:::{{_settings.BucketName}}/*"]
              }]
            }
            """;

        var setPolicyArgs = new SetPolicyArgs()
            .WithBucket(_settings.BucketName)
            .WithPolicy(policy);

        await _minio.SetPolicyAsync(setPolicyArgs, ct);
        _logger.LogInformation("Public read policy applied to bucket '{Bucket}'", _settings.BucketName);
    }
}
