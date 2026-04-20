# Випускник+ (VypusknykPlus) Backend

## Опис проекту

E-commerce платформа для замовлення персоналізованих випускних стрічок, медалей, грамот та аксесуарів для українських шкіл.

## Технології

- **.NET 8.0**, ASP.NET Core Web API, C#
- **PostgreSQL** + Entity Framework Core 8 (Npgsql)
- **JWT Bearer** автентифікація (BCrypt для хешування паролів)
- **MinIO** — S3-compatible object storage для фото продуктів
- **Serilog** — логування (Console + File)
- **FluentValidation** — валідація DTO через ActionFilter
- **Swagger/OpenAPI** для документації

## Структура проекту

```
src/
├── VypusknykPlus.Api/
│   ├── Controllers/
│   │   ├── AuthController.cs           # login, register, refresh, profile, password, forgot/reset
│   │   ├── ProductsController.cs       # GET products (filters), GET by id, POST image
│   │   ├── OrdersController.cs         # POST create, GET user orders, GET by id, guest orders, claim
│   │   ├── DesignsController.cs        # CRUD збережених дизайнів (JWT)
│   │   ├── CartController.cs           # GET, POST, PATCH qty, DELETE item/clear
│   │   ├── AdminAuthController.cs      # POST /admin/auth/login
│   │   ├── AdminUsersController.cs     # GET /admin/users, GET /admin/users/:id
│   │   ├── AdminOrdersController.cs    # GET /admin/orders, GET/:id, PATCH status
│   │   ├── AdminProductsController.cs  # CRUD + images
│   │   ├── AdminDesignsController.cs   # GET /admin/designs
│   │   └── AdminAdminsController.cs    # GET, POST, GET/:id, DELETE /admin/admins
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs  # ArgumentException→400, KeyNotFound→404, Unauthorized→401, else→500
│   └── Program.cs
│
└── VypusknykPlus.Application/
    ├── Entities/
    │   ├── BaseEntity.cs        # Id (long), CreatedAt, UpdatedAt, IsDeleted
    │   ├── User.cs              # Email, FullName, Phone, PasswordHash; nav: Orders, SavedDesigns, CartItems
    │   ├── Admin.cs             # Email, FullName, PasswordHash, LastLoginAt?
    │   ├── Product.cs           # Name, Description, Price, MinOrder, Category, Color, Tags, ImageKey; nav: Images
    │   ├── ProductImage.cs      # ProductId, ImageKey, IsPreview
    │   ├── Order.cs             # OrderNumber, Status, Total, Delivery, Recipient, Payment, IsAnonymous, GuestToken, UserId?
    │   ├── OrderItem.cs         # Name, Quantity, Price, NamesData (JSONB), RibbonCustomization (JSONB)
    │   ├── SavedDesign.cs       # DesignName, SavedAt, State (JSONB), UserId
    │   └── CartItem.cs          # ProductId?, Name, Price, Quantity, UserId
    ├── Migrations/
    │   ├── InitialCreate
    │   ├── AddProductImages
    │   ├── AddOrderItemPersonalization   # NamesData + RibbonCustomization JSONB на OrderItems
    │   └── AddAdminLastLoginAt           # LastLoginAt? на Admin
    ├── Data/
    │   ├── AppDbContext.cs       # Global query filters (!IsDeleted) на: User, Order, OrderItem, SavedDesign, CartItem, Product, Admin
    │   └── Configurations/      # Fluent API конфігурації
    ├── DTOs/
    │   ├── Auth/                # LoginRequest, RegisterRequest, AuthResponse, RefreshToken*, UpdateProfile*, ChangePassword*, ForgotPassword*, ResetPassword*
    │   ├── Admin/
    │   │   ├── AdminLoginRequest / AdminAuthResponse
    │   │   ├── AdminOrderResponse / AdminOrderItemResponse / AdminRecipientResponse / AdminDeliveryResponse
    │   │   ├── AdminProductResponse / AdminProductDetailResponse / ProductImageResponse / SaveProductRequest
    │   │   ├── AdminUserResponse               # Список юзерів (ordersCount)
    │   │   ├── AdminUserDetailResponse         # Деталі юзера + orders[] + savedDesigns[]
    │   │   ├── AdminUserOrderSummary / AdminUserSavedDesign
    │   │   ├── AdminSavedDesignResponse        # Глобальна таблиця дизайнів з user info
    │   │   ├── AdminAdminResponse              # Список адмінів
    │   │   ├── AdminAdminDetailResponse        # Деталі адміна (lastLoginAt)
    │   │   └── CreateAdminRequest
    │   └── PagedResponse.cs     # { items, total, page, pageSize }
    └── Services/
        ├── IAdminService / AdminService    # Всі admin CRUD операції
        ├── IAdminAuthService / AdminAuthService  # Admin login (super admin з env vars + DB admins)
        ├── IAuthService / AuthService      # User auth (login, register, refresh, profile, password, forgot/reset)
        ├── IProductService / ProductService
        ├── IOrderService / OrderService    # create, get, guest orders, claim guest orders
        ├── IDesignService / DesignService
        ├── ICartService / CartService
        ├── IImageService / ImageService    # MinIO upload/delete/getPublicUrl
        └── IEmailService / EmailService    # MailKit SMTP
```

## Admin Auth

Два типи адмінів:
- **Super Admin** — email/пароль з env vars `Admin:Email` / `Admin:Password`, id=0, IsSuperAdmin=true, не в БД
- **DB Admin** — в таблиці `Admins`, BCrypt, `LastLoginAt` оновлюється при кожному логіні

JWT для адмінів видається з роллю `"Admin"` (claim `ClaimTypes.Role`). Всі `/api/v1/admin/*` ендпоінти захищені `[Authorize(Roles = "Admin")]`.

## API ендпоінти

### Публічні / User API

| Метод  | Шлях                              | Опис                              |
|--------|-----------------------------------|-----------------------------------|
| POST   | /api/v1/auth/login                | Логін юзера                       |
| POST   | /api/v1/auth/register             | Реєстрація                        |
| POST   | /api/v1/auth/refresh              | Оновити JWT                       |
| POST   | /api/v1/auth/forgot-password      | Запит відновлення пароля          |
| POST   | /api/v1/auth/reset-password       | Скинути пароль                    |
| PUT    | /api/v1/auth/profile              | Оновити профіль (JWT)             |
| PUT    | /api/v1/auth/password             | Змінити пароль (JWT)              |
| GET    | /api/v1/products                  | Список продуктів (фільтри)        |
| GET    | /api/v1/products/{id}             | Деталі продукту                   |
| POST   | /api/v1/orders                    | Створити замовлення (-/JWT)       |
| GET    | /api/v1/orders                    | Замовлення юзера (JWT)            |
| GET    | /api/v1/orders/{id}               | Деталі замовлення (JWT)           |
| GET    | /api/v1/orders/guest/{token}      | Замовлення гостя                  |
| POST   | /api/v1/orders/claim              | Прив'язати гостьові (JWT)         |
| POST/GET/PUT/DELETE | /api/v1/designs        | CRUD збережених дизайнів (JWT)    |
| GET/POST/PATCH/DELETE | /api/v1/cart         | Кошик (JWT)                       |

### Admin API (`[Authorize(Roles = "Admin")]`)

| Метод  | Шлях                                                  | Опис                              |
|--------|-------------------------------------------------------|-----------------------------------|
| POST   | /api/v1/admin/auth/login                              | Логін адміна                      |
| GET    | /api/v1/admin/orders                                  | Всі замовлення (paginated)        |
| GET    | /api/v1/admin/orders/{id}                             | Деталі замовлення                 |
| PATCH  | /api/v1/admin/orders/{id}/status                      | Оновити статус                    |
| GET    | /api/v1/admin/products                                | Всі продукти (з IsDeleted)        |
| GET    | /api/v1/admin/products/{id}                           | Деталі + images[]                 |
| POST   | /api/v1/admin/products                                | Створити продукт                  |
| PUT    | /api/v1/admin/products/{id}                           | Оновити продукт                   |
| DELETE | /api/v1/admin/products/{id}                           | Soft delete                       |
| POST   | /api/v1/admin/products/{id}/images                    | Завантажити фото (multipart)      |
| DELETE | /api/v1/admin/products/{id}/images/{imageId}          | Видалити фото                     |
| PATCH  | /api/v1/admin/products/{id}/images/{imageId}/preview  | Встановити превʼю                 |
| GET    | /api/v1/admin/users                                   | Всі юзери (paginated)             |
| GET    | /api/v1/admin/users/{id}                              | Деталі юзера + orders + designs   |
| GET    | /api/v1/admin/designs                                 | Всі збережені дизайни (paginated) |
| GET    | /api/v1/admin/admins                                  | Список адмінів (paginated)        |
| GET    | /api/v1/admin/admins/{id}                             | Деталі адміна                     |
| POST   | /api/v1/admin/admins                                  | Створити адміна                   |
| DELETE | /api/v1/admin/admins/{id}                             | Soft delete адміна                |

## Важливі патерни

- **EF Core JSONB**: `OwnsOne(e => e.Field, b => { b.ToJson(); })` — NamesData, RibbonCustomization на OrderItem; RibbonState на SavedDesign
- **Global query filters**: `HasQueryFilter(e => !e.IsDeleted)` на всіх сутностях. В адмін-сервісах використовувати `IgnoreQueryFilters()` де потрібно бачити soft-deleted записи
- **Soft delete**: IsDeleted = true + UpdatedAt = UtcNow. Ніколи не видаляти фізично
- **Middleware order**: `UseSerilogRequestLogging()` ДО `UseMiddleware<ExceptionHandlingMiddleware>()`
- **Claim guest orders**: `ClaimGuestOrdersAsync` викликається fire-and-forget при логіні/реєстрації юзера

## Конфігурація

- **БД**: `Host=localhost;Port=5432;Database=vypusknyk_plus;Username=postgres;Password=postgres` (dev)
- **JWT**: `Jwt__Key`, expiration 15 хв (user), 8 год (admin)
- **Admin**: `Admin:Email`, `Admin:Password` (super admin з env)
- **CORS**: `Cors__AllowedOrigins`
- **MinIO**: `Minio__Endpoint`, `Minio__AccessKey`, `Minio__SecretKey`, `MINIO_PUBLIC_ENDPOINT`
- **Email**: SMTP через `Email__*` (Gmail App Password)
- **Порт**: HTTP `localhost:5272` (dev), `8080` (Docker)

## Команди

```bash
# Запуск
dotnet run --project src/VypusknykPlus.Api

# Міграції
dotnet ef migrations add <Name> --project src/VypusknykPlus.Application --startup-project src/VypusknykPlus.Api
dotnet ef database update --project src/VypusknykPlus.Application --startup-project src/VypusknykPlus.Api

# Docker (prod)
docker buildx build --platform linux/amd64 -t stepll/vypusknyk-plus:latest --push .
# На сервері: docker compose pull && docker compose up -d
```

## Деплой

- Сервер: `vmi3229320` (IP: `75.119.152.4`), Contabo VPS
- Docker Compose: api + db (PostgreSQL) + minio
- **Автоміграції**: `MigrateAsync()` в `Program.cs` при старті
- Nginx: reverse proxy + HTTPS (sslip.io), `client_max_body_size 15m`, `/storage/` → MinIO порт 9000

## TODO

- [ ] Деплой бекенду з міграціями `AddProductImages`, `AddOrderItemPersonalization`, `AddAdminLastLoginAt`
- [ ] CORS: додати admin Vercel URL до `Cors:AllowedOrigins`
- [ ] Домен: купити домен, оновити Nginx + Let's Encrypt
- [ ] Зображення продуктів: завантажити реальні фото через admin panel
