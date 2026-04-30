using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public interface IPageContentService
{
    Task<string?> GetDataAsync(string slug);
    Task<string> UpsertDataAsync(string slug, string data);
    Task<string> UploadImageAsync(string slug, string field, Stream stream, string contentType);
}

public class PageContentService : IPageContentService
{
    private readonly AppDbContext _db;
    private readonly IImageService _imageService;

    public PageContentService(AppDbContext db, IImageService imageService)
    {
        _db = db;
        _imageService = imageService;
    }

    public async Task<string?> GetDataAsync(string slug)
    {
        var record = await _db.PageContents.FindAsync(slug);
        return record?.Data;
    }

    public async Task<string> UpsertDataAsync(string slug, string data)
    {
        var record = await _db.PageContents.FindAsync(slug);
        if (record is null)
        {
            record = new PageContent { Slug = slug };
            _db.PageContents.Add(record);
        }
        record.Data = data;
        record.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return record.Data;
    }

    public async Task<string> UploadImageAsync(string slug, string field, Stream stream, string contentType)
    {
        var ext = contentType switch
        {
            "image/jpeg" => "jpg",
            "image/png" => "png",
            "image/webp" => "webp",
            _ => "jpg"
        };
        var key = $"page-content/{slug}/{field}.{ext}";
        await _imageService.UploadAsync(key, stream, contentType);
        return _imageService.GetPublicUrl(key) ?? key;
    }
}
