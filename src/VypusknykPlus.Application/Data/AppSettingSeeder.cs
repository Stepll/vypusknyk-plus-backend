using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data;

public static class AppSettingSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.AppSettings.AnyAsync()) return;

        var now = DateTime.UtcNow;

        var settings = new List<AppSetting>
        {
            // orders
            new() { Key = "min_order_amount",        Value = "500",   Type = "number",  Group = "orders",      Label = "Мінімальна сума замовлення",           Description = "Мінімальна сума в гривнях для оформлення замовлення", UpdatedAt = now },
            new() { Key = "free_delivery_threshold", Value = "1500",  Type = "number",  Group = "orders",      Label = "Безкоштовна доставка від",             Description = "Сума замовлення від якої доставка безкоштовна", UpdatedAt = now },
            new() { Key = "production_days",         Value = "7",     Type = "number",  Group = "orders",      Label = "Орієнтовний термін виготовлення",       Description = "Стандартна кількість робочих днів на виготовлення", UpdatedAt = now },
            new() { Key = "production_days_peak",    Value = "14",    Type = "number",  Group = "orders",      Label = "Термін виготовлення (сезонний)",        Description = "Кількість днів у пік сезону (травень–червень)", UpdatedAt = now },

            // store
            new() { Key = "catalog_enabled",             Value = "true",                                            Type = "boolean", Group = "store", Label = "Каталог увімкнено",                   UpdatedAt = now },
            new() { Key = "constructor_enabled",         Value = "true",                                            Type = "boolean", Group = "store", Label = "Конструктор увімкнено",               UpdatedAt = now },
            new() { Key = "online_payment_enabled",      Value = "false",                                           Type = "boolean", Group = "store", Label = "Онлайн-оплата увімкнена",             UpdatedAt = now },
            new() { Key = "peak_season_mode",            Value = "false",                                           Type = "boolean", Group = "store", Label = "Режим сезонного навантаження",        UpdatedAt = now },
            new() { Key = "peak_season_banner_text",     Value = "Зараз підвищений попит. Терміни виготовлення збільшені.", Type = "string", Group = "store", Label = "Текст банера сезонного навантаження", UpdatedAt = now },
            new() { Key = "maintenance_mode",            Value = "false",                                           Type = "boolean", Group = "store", Label = "Технічні роботи",                     UpdatedAt = now },
            new() { Key = "maintenance_text",            Value = "Сайт тимчасово недоступний. Спробуйте пізніше.", Type = "string",  Group = "store", Label = "Текст сторінки технічних робіт",      UpdatedAt = now },

            // ribbon
            new() { Key = "ribbon_max_text_length",      Value = "30", Type = "number", Group = "ribbon", Label = "Макс. символів основного тексту",     UpdatedAt = now },
            new() { Key = "ribbon_max_school_length",    Value = "50", Type = "number", Group = "ribbon", Label = "Макс. символів назви школи",           UpdatedAt = now },
            new() { Key = "ribbon_max_classes",          Value = "20", Type = "number", Group = "ribbon", Label = "Макс. кількість класів в замовленні",  UpdatedAt = now },
            new() { Key = "ribbon_max_names_per_class",  Value = "40", Type = "number", Group = "ribbon", Label = "Макс. імен в одному класі",            UpdatedAt = now },

            // badge
            new() { Key = "badge_max_top_text_length",    Value = "25", Type = "number", Group = "badge", Label = "Макс. символів верхнього тексту",  UpdatedAt = now },
            new() { Key = "badge_max_bottom_text_length", Value = "25", Type = "number", Group = "badge", Label = "Макс. символів нижнього тексту",   UpdatedAt = now },

            // certificate
            new() { Key = "certificate_max_title_length", Value = "80",  Type = "number", Group = "certificate", Label = "Макс. символів заголовку",        UpdatedAt = now },
            new() { Key = "certificate_max_body_length",  Value = "300", Type = "number", Group = "certificate", Label = "Макс. символів основного тексту", UpdatedAt = now },

            // contacts
            new() { Key = "contact_phone",    Value = "+380 (67) 671-45-10",                  Type = "string", Group = "contacts", Label = "Телефон",          UpdatedAt = now },
            new() { Key = "contact_email",    Value = "vypusk.org@gmail.com",                 Type = "string", Group = "contacts", Label = "Email",             UpdatedAt = now },
            new() { Key = "working_hours",    Value = "Пн–Пт, 9:00–18:00",                   Type = "string", Group = "contacts", Label = "Робочі години",     UpdatedAt = now },
            new() { Key = "social_instagram", Value = "https://www.instagram.com/vypusk_org/", Type = "string", Group = "contacts", Label = "Instagram URL",    UpdatedAt = now },
            new() { Key = "social_viber",     Value = "+380676714510",                         Type = "string", Group = "contacts", Label = "Viber номер",      UpdatedAt = now },
            new() { Key = "social_telegram",  Value = "+380676714510",                         Type = "string", Group = "contacts", Label = "Telegram",         UpdatedAt = now },
        };

        db.AppSettings.AddRange(settings);
        await db.SaveChangesAsync();
    }
}
