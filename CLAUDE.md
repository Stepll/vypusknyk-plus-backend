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
│   │   ├── AuthController.cs
│   │   ├── ProductsController.cs
│   │   ├── OrdersController.cs
│   │   ├── DesignsController.cs
│   │   ├── CartController.cs
│   │   ├── AdminAuthController.cs
│   │   ├── AdminUsersController.cs
│   │   ├── AdminOrdersController.cs
│   │   ├── AdminProductsController.cs
│   │   ├── AdminDesignsController.cs
│   │   ├── AdminAdminsController.cs
│   │   ├── AdminWarehouseController.cs   # GET stats/categories/subcategories/products, POST transactions/products
│   │   ├── AdminDeliveriesController.cs  # GET/POST deliveries, receive item, receive-all (→204)
│   │   └── AdminSuppliersController.cs   # CRUD постачальників
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs  # ArgumentException→400, KeyNotFound→404, else→500
│   └── Program.cs
│
└── VypusknykPlus.Application/
    ├── Entities/
    │   ├── BaseEntity.cs          # Id (long), CreatedAt, UpdatedAt, IsDeleted
    │   ├── User.cs
    │   ├── Admin.cs               # LastLoginAt?, RoleId? FK → Role
    │   ├── Role.cs                # Name, Color, Pages (text[]), IsSuperAdmin; soft delete
    │   ├── Product.cs
    │   ├── ProductImage.cs
    │   ├── Order.cs
    │   ├── OrderItem.cs           # NamesData (JSONB), RibbonCustomization (JSONB)
    │   ├── SavedDesign.cs
    │   ├── CartItem.cs
    │   ├── StockCategory.cs       # Id, Name, Order
    │   ├── StockSubcategory.cs    # Id, CategoryId, Name, Order
    │   ├── StockProduct.cs        # SubcategoryId, Name, HasColor, HasMaterial
    │   ├── StockVariant.cs        # ProductId, Material, Color; унікальний індекс (ProductId, Material, Color)
    │   ├── StockTransaction.cs    # VariantId, DeliveryItemId?, OrderId?, Type, Quantity, Date, Note
    │   ├── Supplier.cs            # Name, ContactPerson?, Phone?, Email?, TaxId?, Address?, Notes?, IsDeleted
    │   ├── Delivery.cs            # Number (DEL-YYYY-NNNN), SupplierId?, ExpectedDate, Status, IsDeleted
    │   └── DeliveryItem.cs        # DeliveryId, ProductId, Material, Color, ExpectedQty, ReceivedQty, ReceivedAt?
    ├── Migrations/
    │   ├── InitialCreate
    │   ├── AddProductImages
    │   ├── AddOrderItemPersonalization
    │   ├── AddAdminLastLoginAt
    │   ├── AddWarehouseManagement           # StockCategory/Subcategory/Product/Variant/Transaction + seed 28 products
    │   ├── RemoveStockVariantCurrentStock   # CurrentStock видалено — обчислюється з транзакцій
    │   ├── AddStockSubcategoryAndProductParams  # HasColor, HasMaterial + subcategory hierarchy
    │   ├── FixStockProductIdAutoGenerate    # Виправляє sequence після explicit-ID seeding
    │   ├── AddDeliveriesAndSuppliers        # Supplier, Delivery, DeliveryItem + DeliveryItemId на StockTransaction
    │   ├── AddSupplierExtraFields           # TaxId, Address, Notes на Supplier
    │   ├── AddStockTransactionOrderId       # OrderId на StockTransaction
    │   └── AddRoles                         # Role (text[] Pages), Admin.RoleId FK; seed SuperAdmin/Manager/Warehouse
    ├── Data/
    │   ├── AppDbContext.cs        # Global query filters (!IsDeleted) на: User, Order, OrderItem,
    │   │                          # SavedDesign, CartItem, Product, Admin, Supplier, Delivery
    │   └── Configurations/
    ├── DTOs/Admin/
    │   ├── WarehouseDtos.cs       # StockCategoryResponse, StockProductSummary, StockProductDetail,
    │   │                          # StockVariantResponse, StockTransactionResponse (deliveryId?, orderId?,
    │   │                          # orderNumber?, orderCreatedAt?), WarehouseStats,
    │   │                          # CreateStockTransactionRequest (orderId?), CreateStockProductRequest
    │   └── DeliveryDtos.cs        # SupplierResponse, SaveSupplierRequest, DeliverySummary, DeliveryDetail,
    │                              # DeliveryItemResponse (receiveHistory: ReceiveTransactionInfo[]),
    │                              # ReceiveTransactionInfo, CreateDeliveryRequest, ReceiveDeliveryItemRequest,
    │                              # ReceiveAllRequest, DeliveryQuery
    └── Services/
        ├── IWarehouseService / WarehouseService
        │   # GetStats, GetCategories, GetSubcategories, GetProducts, GetProductDetail,
        │   # AddTransaction (зберігає OrderId, резолвує orderNumber/orderCreatedAt),
        │   # CreateProduct
        │   # CurrentStock = SUM(income) - SUM(outcome) по StockTransactions
        │   # GetProductDetail: резолвує DeliveryId (через DeliveryItems) і OrderId (через Orders) для транзакцій
        └── IDeliveryService / DeliveryService
            # GetSuppliers/Create/Update/Delete (soft)
            # GetDeliveries, GetDeliveryDetail (ThenInclude Transactions → ReceiveHistory)
            # CreateDelivery (номер DEL-YYYY-NNNN через COUNT+1, unique index захищає)
            # ReceiveItemAsync: Delivery завантажується окремо через IgnoreQueryFilters()
            #   щоб уникнути null від global query filter при Include
            # ReceiveAllAsync: IgnoreQueryFilters()
            # FindOrCreateVariantAsync: приватний helper
```

## Admin Auth

Два типи адмінів:
- **Super Admin** — env vars `Admin:Email` / `Admin:Password`, id=0, не в БД, завжди повний доступ
- **DB Admin** — таблиця `Admins`, BCrypt, `LastLoginAt` оновлюється при логіні, має `RoleId FK → Roles`

JWT з роллю `"Admin"` + custom claims: `roleId`, `roleName`, `roleColor`, `isSuperAdmin`, `pages` (JSON array).
Всі `/api/v1/admin/*` захищені `[Authorize(Roles = "Admin")]`.

**Система ролей:**
- `Role`: Name, Color, Pages (text[]), IsSuperAdmin, soft delete
- 3 дефолтних ролі (seed у міграції): SuperAdmin (isSuperAdmin=true), Manager, Warehouse
- SuperAdmin роль захищена від PUT/DELETE на рівні сервісу (`InvalidOperationException`)
- `AdminAuthResponse` містить `RoleInfo?` з id/name/color/isSuperAdmin/pages

## API ендпоінти (Admin)

### Замовлення / Продукти / Юзери / Адміни — стандартні CRUD (незмінні)

### Адміни та Ролі
| Метод | Шлях | Опис |
|-------|------|------|
| GET | /api/v1/admin/admins | Список (paginated, Include Role) |
| GET | /api/v1/admin/admins/{id} | Деталі (Include Role) |
| POST | /api/v1/admin/admins | Створити (з RoleId?) |
| DELETE | /api/v1/admin/admins/{id} | Soft delete |
| PATCH | /api/v1/admin/admins/{id}/password | Змінити пароль → 204 |
| PATCH | /api/v1/admin/admins/{id}/role | Змінити роль → AdminAdminDetailResponse |
| GET | /api/v1/admin/roles | Список ролей |
| POST | /api/v1/admin/roles | Створити роль |
| PUT | /api/v1/admin/roles/{id} | Оновити (SuperAdmin → 400) |
| DELETE | /api/v1/admin/roles/{id} | Soft delete (SuperAdmin → 400) |

### Складський облік
| Метод | Шлях | Опис |
|-------|------|------|
| GET | /api/v1/admin/warehouse/stats | Статистика |
| GET | /api/v1/admin/warehouse/categories | Категорії |
| GET | /api/v1/admin/warehouse/subcategories | Підкатегорії |
| GET | /api/v1/admin/warehouse/products | Список (paginated) |
| GET | /api/v1/admin/warehouse/products/{id} | Деталі + варіанти + транзакції |
| POST | /api/v1/admin/warehouse/products | Створити товар |
| POST | /api/v1/admin/warehouse/transactions | Додати прихід/видачу |

### Постачальники
| Метод | Шлях | Опис |
|-------|------|------|
| GET | /api/v1/admin/suppliers | Список |
| POST | /api/v1/admin/suppliers | Створити |
| PUT | /api/v1/admin/suppliers/{id} | Оновити |
| DELETE | /api/v1/admin/suppliers/{id} | Soft delete |

### Поставки
| Метод | Шлях | Опис |
|-------|------|------|
| GET | /api/v1/admin/deliveries | Список (paginated, фільтри) |
| GET | /api/v1/admin/deliveries/{id} | Деталі + items[].receiveHistory[] |
| POST | /api/v1/admin/deliveries | Створити |
| POST | /api/v1/admin/deliveries/{id}/items/{itemId}/receive | Прийняти позицію |
| POST | /api/v1/admin/deliveries/{id}/receive-all | Прийняти всі → **204 NoContent** |

## Важливі патерни

- **Global query filters** на Delivery і Supplier (як на всіх інших). При завантаженні через Include EF Core застосовує фільтр до JOIN — якщо є ризик null, завантажувати окремо з `IgnoreQueryFilters()`.
- **EF Core JSONB**: `OwnsOne(e => e.Field, b => { b.ToJson(); })` — NamesData, RibbonCustomization
- **Soft delete**: `IsDeleted = true`, ніколи не видаляти фізично
- **StockProduct seed** (ID 1–28 явні): після будь-якого re-seeding потрібна міграція `setval(pg_get_serial_sequence('"StockProducts"', 'Id'), MAX(Id), true)`
- **CurrentStock**: `SUM(income qty) - SUM(outcome qty)` з StockTransactions. Поле `CurrentStock` на `StockVariant` — видалено.
- **Delivery number**: `DEL-{year}-{COUNT+1:D4}`. Unique index на Number захищає від дублів.
- **ReceiveAll** повертає `NoContent()` (204) — не `Ok()`.

## Конфігурація

- **БД** (dev): `Host=localhost;Port=5432;Database=vypusknyk_plus;Username=postgres;Password=postgres`
- **JWT**: `Jwt__Key`, 15 хв (user), 8 год (admin)
- **Admin**: `Admin:Email`, `Admin:Password`
- **CORS**: `Cors__AllowedOrigins`
- **MinIO**: `Minio__Endpoint`, `Minio__AccessKey`, `Minio__SecretKey`, `MINIO_PUBLIC_ENDPOINT`
- **Порт**: `5272` (dev), `8080` (Docker)

## Команди

```bash
# Запуск
dotnet run --project src/VypusknykPlus.Api

# Міграції (з src/VypusknykPlus.Api щоб env vars завантажились)
cd src/VypusknykPlus.Api
dotnet ef migrations add <Name> --project ../VypusknykPlus.Application
dotnet ef database update --project ../VypusknykPlus.Application

# Docker (prod)
docker buildx build --platform linux/amd64 -t stepll/vypusknyk-plus:latest --push .
# На сервері:
cd ~/vypusknyk-plus/prod && docker compose pull && docker compose up -d
docker logs prod-api-1 --tail 50
```

## Деплой

- Сервер: `vmi3229320` (IP: `75.119.152.4`), Contabo VPS
- Docker Compose: api + db (PostgreSQL) + minio
- **Автоміграції**: `MigrateAsync()` в `Program.cs` при старті
- Nginx: reverse proxy + HTTPS (sslip.io), `client_max_body_size 15m`, `/storage/` → MinIO :9000

**Скинути БД на сервері:**
```bash
docker stop prod-api-1
docker exec prod-db-1 psql -U postgres -c "DROP DATABASE vypusknyk_plus;"
docker exec prod-db-1 psql -U postgres -c "CREATE DATABASE vypusknyk_plus;"
docker start prod-api-1
```

## TODO

- [ ] CORS: додати admin Vercel URL до `Cors:AllowedOrigins`
- [ ] Домен: купити домен, оновити Nginx + Let's Encrypt
- [ ] Зображення продуктів: завантажити реальні фото через admin panel
