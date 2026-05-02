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
│   │   ├── AdminSuppliersController.cs   # CRUD постачальників
│   │   ├── AdminAuditLogsController.cs   # GET /api/v1/admin/audit-logs (?entityTypes[], entityId, adminId, action, from, to, page, pageSize)
│   │   ├── AdminRibbonPrintTypesController.cs  # CRUD /admin/ribbon-print-types
│   │   ├── AdminRibbonEmblemsController.cs     # CRUD /admin/ribbon-emblems + POST svg/left, POST svg/right
│   │   ├── AdminConstructorRulesController.cs  # CRUD /admin/constructor-rules/incompatibilities + /forced-texts
│   │   ├── RibbonPrintTypesController.cs       # Public GET /api/v1/ribbon-print-types
│   │   ├── RibbonEmblemsController.cs          # Public GET /api/v1/ribbon-emblems
│   │   └── ConstructorRulesController.cs       # Public GET /api/v1/constructor/rules
│   ├── Infrastructure/
│   │   └── CurrentAdminProvider.cs  # ICurrentAdminProvider impl — читає adminId/adminName з IHttpContextAccessor claims
│   │                                 # Super Admin: sub=0, name з env; DB admin: sub=adminId, name=FullName
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs  # ArgumentException→400, KeyNotFound→404, else→500
│   └── Program.cs  # AddHttpContextAccessor, AddScoped<ICurrentAdminProvider>, AddScoped<AuditInterceptor>
│                    # AddDbContext factory pattern: (sp, options) => options.UseNpgsql(...).AddInterceptors(sp.GetRequiredService<AuditInterceptor>())
│
└── VypusknykPlus.Application/
    ├── Entities/
    │   ├── BaseEntity.cs          # Id (long), CreatedAt, UpdatedAt, IsDeleted
    │   ├── AuditLog.cs            # NOT BaseEntity; AdminId (long?, nullable, no FK), AdminName (string),
    │   │                          # EntityType (string), EntityId (long), Action (string),
    │   │                          # ChangesJson (string?), CreatedAt (DateTime UTC)
    │   ├── User.cs                # + IsEmailVerified, IsNameVerified, IsPhoneVerified (bool)
    │   │                          # + GoogleId (string?, nullable) — встановлюється при Google OAuth
    │   ├── EmailVerificationToken.cs  # Token (string), ExpiresAt, IsUsed, UserId FK; IsValid computed
    │   ├── Admin.cs               # LastLoginAt?, RoleId? FK → Role
    │   ├── Role.cs                # Name, Color, Pages (text[]), IsSuperAdmin; soft delete
    │   ├── Product.cs             # CategoryId FK → ProductCategory, SubcategoryId? FK → ProductSubcategory
    │   ├── ProductCategory.cs     # Id, Name, Order; seed: Стрічки(1)/Медалі(2)/Грамоти(3)/Аксесуари(4)
    │   ├── ProductSubcategory.cs  # Id, CategoryId, Name, Order; ValueGeneratedOnAdd()
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
    │   ├── DeliveryItem.cs        # DeliveryId, ProductId, Material, Color, ExpectedQty, ReceivedQty, ReceivedAt?
    │   ├── RibbonPrintType.cs     # Name, Slug, PriceModifier, IsActive, SortOrder
    │   ├── RibbonEmblem.cs        # Name, Slug, SvgKeyLeft?, SvgKeyRight?, IsActive, SortOrder
    │   ├── ConstructorIncompatibility.cs  # TypeA, SlugA, TypeB, IsWarning, Message; Targets[]
    │   ├── ConstructorIncompatibilityTarget.cs  # RuleId FK, SlugB
    │   ├── ConstructorForcedText.cs       # TriggerType, TriggerSlug, TargetField, Message; Values[]
    │   ├── ConstructorForcedTextValue.cs  # RuleId FK, Value
    │   ├── NotificationTriggerConfig.cs   # TriggerType (PK string), EmailEnabled/Recipients/Subject/Message,
    │   │                                  # TelegramEnabled/UserIds/GroupEnabled/Message,
    │   │                                  # SystemEnabled/AdminIds/Title/Message; Recipients/UserIds/AdminIds = JSON
    │   └── AdminNotification.cs           # AdminId FK (long), TriggerType, Title, Body, EntityType?, EntityId?,
    │                                      # IsRead, CreatedAt
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
    │   ├── AddRoles                         # Role (text[] Pages), Admin.RoleId FK; seed SuperAdmin/Manager/Warehouse
    │   ├── AddProductCategoriesTable        # ProductCategories/Subcategories; data-міграція Category string→CategoryId FK
    │   ├── AddRibbonPrintTypesTable         # RibbonPrintTypes + seed: foil/film/3d
    │   ├── AddRibbonEmblemsTable            # RibbonEmblems + seed: bell(0)/star(1)/diploma(2)/heart(3)/torch(4)/star-3d(5)
    │   ├── SplitEmblemSvgKeys              # DROP SvgKey → ADD SvgKeyLeft + SvgKeyRight
    │   ├── AddConstructorRulesTables       # ConstructorIncompatibilities + Targets + ForcedTexts + Values (CASCADE)
    │   ├── AddUserVerificationFields       # IsEmailVerified, IsNameVerified, IsPhoneVerified на User
    │   ├── AddEmailVerificationToken       # EmailVerificationTokens таблиця (Token, ExpiresAt, IsUsed, UserId FK)
    │   ├── AddNotifications                # NotificationTriggerConfigs (PK string) + AdminNotifications (AdminId FK)
    │   ├── AddNotificationTemplates        # +5 колонок на NotificationTriggerConfigs: SystemTitle, SystemMessage,
    │   │                                   # EmailSubject, EmailMessage, TelegramMessage
    │   ├── AddAuditLogs                    # AuditLogs table (AdminId long? NO FK, AdminName, EntityType, EntityId,
    │   │                                   # Action, ChangesJson, CreatedAt); indexes на EntityType+EntityId, AdminId, CreatedAt
    │   └── AddGoogleIdToUser               # GoogleId (text, nullable) на Users
    ├── Controllers/
    │   ├── AdminProductCategoriesController.cs  # [Route("api/v1/admin/product-categories")] CRUD + subcategories
    │   └── ProductCategoriesController.cs       # Public GET /api/v1/product-categories
    ├── Data/
    │   ├── AppDbContext.cs        # Global query filters (!IsDeleted) на: User, Order, OrderItem,
    │   │                          # SavedDesign, CartItem, Product, Admin, Supplier, Delivery
    │   ├── AuditInterceptor.cs    # SaveChangesInterceptor — трекає зміни 22 типів сутностей
    │   │                          # SavingChangesAsync: CaptureChanges → _pending list
    │   │                          # SavedChangesAsync: Detach non-AuditLog entities → INSERT AuditLogs
    │   │                          # TrackedTypes (22): Order, Product, User, Admin, Role, Delivery, Supplier,
    │   │                          #   ProductCategory, ProductSubcategory, StockProduct, DeliveryMethod,
    │   │                          #   PaymentMethod, OrderStatus, NotificationTriggerConfig,
    │   │                          #   RibbonColor, RibbonMaterial, RibbonPrintColor, RibbonFont,
    │   │                          #   RibbonPrintType, RibbonEmblem, ConstructorIncompatibility, ConstructorForcedText
    │   │                          # MergeChildChanges<T>: ConstructorIncompatibilityTarget/ForcedTextValue → parent entry
    │   │                          # ВАЖЛИВО: Detach перед 2-м SaveChanges → запобігає re-save tracked entities
    │   ├── ICurrentAdminProvider.cs  # Interface (Application layer): (long? AdminId, string AdminName) GetCurrent()
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
    ├── DTOs/Admin/
    │   ├── ProductCategoryDtos.cs   # ProductCategoryResponse, ProductSubcategoryResponse, SaveProductCategoryRequest
    │   └── DashboardChartResponse.cs # (розширено) SalesByCategoryResponse, SalesCategoryEntry,
    │                                  # SalesSubcategoryEntry, SalesProductEntry
    └── Services/
        ├── IAuditLogService / AuditLogService
        │   # GetLogsAsync(entityTypes[]?, entityId?, adminId?, action?, from?, to?, page, pageSize)
        │   # entityTypes — масив для multi-filter (використовує .Contains)
        │   # Повертає PagedResponse<AuditLogResponse>
        ├── IEmailService / EmailService   # SendPasswordResetEmailAsync, SendOrderConfirmationEmailAsync,
        │                                  # SendActivationEmailAsync (брендований HTML), SendRawEmailAsync
        ├── IAuthService / AuthService     # + VerifyEmailAsync(token), ResendActivationEmailAsync(userId)
        │                                  # + GoogleLoginAsync(GoogleLoginRequest) → POST /api/v1/auth/google
        │                                  #   Верифікує access_token через GET googleapis.com/oauth2/v3/userinfo
        │                                  #   Знаходить за GoogleId → або за Email → або створює нового юзера
        │                                  #   IsEmailVerified=true для нових Google-юзерів
        │                                  #   ВАЖЛИВО: ToAuthResponse() мусить бути ПЕРЕД fire-and-forget задачами
        │                                  #   (інакше DbContext concurrency error)
        │                                  # RegisterAsync: fire-and-forget SendActivationEmailInBackgroundAsync
        │                                  #   + ClaimGuestOrdersInBackgroundAsync — обидва через IServiceScopeFactory
        │                                  # ВАЖЛИВО: fire-and-forget мусить створювати scope через
        │                                  #   IServiceScopeFactory.CreateAsyncScope() — інакше DbContext concurrency
        ├── IAdminService / AdminService   # + PatchUserInfoAsync, PatchUserVerificationAsync,
        │                                  # SendUserActivationEmailAsync, SendUserEmailAsync
        ├── IProductCategoryService / ProductCategoryService  # CRUD ProductCategories + Subcategories
        ├── IWarehouseService / WarehouseService
        │   # GetStats, GetCategories, GetSubcategories, GetProducts, GetProductDetail,
        │   # AddTransaction (зберігає OrderId, резолвує orderNumber/orderCreatedAt),
        │   # CreateProduct
        │   # CurrentStock = SUM(income) - SUM(outcome) по StockTransactions
        │   # GetProductDetail: резолвує DeliveryId (через DeliveryItems) і OrderId (через Orders) для транзакцій
        ├── IDashboardService / DashboardService
        │   # GetSalesByCategoryAsync: агрегує OrderItems по ProductName → join in-memory до Products
        │   # (OrderItem не має ProductId FK — тільки Name string). period: week/month/year/all
        ├── INotificationService / NotificationService
        │   # OnNewOrderAsync(orderId, orderNumber, context) — тригер "new_order"
        │   # OnOrderStatusChangedAsync(orderId, orderNumber, newStatusName, context) — тригер "order_status_changed"
        │   #   + "order_status_changed:{statusName}" (обидва незалежно)
        │   # OnNewUserAsync(userId, context) — тригер "new_user"
        │   # context = Dictionary<string,string> — передається з caller-а, сервіс додає orderUrl/userUrl
        │   # ApplyTemplate: {{variable}} підстановка; OrDefault: fallback якщо шаблон порожній
        │   # System канал: DispatchAsync → INSERT AdminNotifications + SignalR PushToAdminAsync
        │   # Email канал: SendEmailsAsync → SendRawEmailAsync до кожного отримувача
        │   # Telegram — конфіг зберігається, відправки немає (не реалізовано)
        │   # Super Admin (id=0): DispatchAsync пропускає DB INSERT, тільки SignalR push
        │   # GetTriggerConfigsAsync, UpdateTriggerConfigAsync, GetMyNotificationsAsync,
        │   # MarkReadAsync, MarkAllReadAsync, GetUnreadCountAsync
        ├── INotificationPushService / SignalRNotificationPushService
        │   # PushToAdminAsync(adminId, dto) → Hub.Clients.Group("admin:{id}").SendAsync("ReceiveAdminNotification")
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

### Auth (публічні + авторизовані)
| Метод | Шлях | Опис |
|-------|------|------|
| POST | /api/v1/auth/google | Google OAuth login/register ({ accessToken }) → AuthResponse |
| GET | /api/v1/auth/verify-email?token= | Підтвердити email за токеном → 200/400 |
| POST | /api/v1/auth/resend-activation | Повторно надіслати лист активації (Authorize) → 204 |

### Юзери (адмін)
| Метод | Шлях | Опис |
|-------|------|------|
| GET | /api/v1/admin/users | Список (paginated) |
| GET | /api/v1/admin/users/{id} | Деталі + orders[] + savedDesigns[] |
| PATCH | /api/v1/admin/users/{id}/info | Оновити fullName/phone → AdminUserDetailResponse |
| PATCH | /api/v1/admin/users/{id}/verification | Оновити isEmailVerified/isNameVerified/isPhoneVerified |
| POST | /api/v1/admin/users/{id}/send-activation-email | Надіслати лист активації → 204 |
| POST | /api/v1/admin/users/{id}/send-email | Надіслати кастомний лист (subject, body) → 204 |

### Замовлення / Продукти / Адміни — стандартні CRUD (незмінні)

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

### Категорії продуктів
| Метод | Шлях | Опис |
|-------|------|------|
| GET | /api/v1/product-categories | Публічний список (з підкатегоріями) |
| GET | /api/v1/admin/product-categories | Адмін список |
| POST | /api/v1/admin/product-categories | Створити категорію |
| PUT | /api/v1/admin/product-categories/{id} | Оновити |
| DELETE | /api/v1/admin/product-categories/{id} | Видалити |
| POST | /api/v1/admin/product-categories/{id}/subcategories | Створити підкатегорію |
| PUT | /api/v1/admin/product-categories/{catId}/subcategories/{id} | Оновити підкатегорію |
| DELETE | /api/v1/admin/product-categories/{catId}/subcategories/{id} | Видалити підкатегорію |

### Дашборд (додаткові ендпоінти)
| Метод | Шлях | Опис |
|-------|------|------|
| GET | /api/v1/admin/dashboard/sales-by-category?period= | Продажі за категоріями (week/month/year/all) |

### Конструктор — типи друку
| Метод | Шлях | Опис |
|-------|------|------|
| GET | /api/v1/ribbon-print-types | Публічний список активних |
| GET | /api/v1/admin/ribbon-print-types | Адмін список усіх |
| POST | /api/v1/admin/ribbon-print-types | Створити |
| PUT | /api/v1/admin/ribbon-print-types/{id} | Оновити |
| DELETE | /api/v1/admin/ribbon-print-types/{id} | Soft delete |

### Конструктор — емблеми
| Метод | Шлях | Опис |
|-------|------|------|
| GET | /api/v1/ribbon-emblems | Публічний список активних |
| GET | /api/v1/admin/ribbon-emblems | Адмін список усіх |
| POST | /api/v1/admin/ribbon-emblems | Створити |
| PUT | /api/v1/admin/ribbon-emblems/{id} | Оновити |
| DELETE | /api/v1/admin/ribbon-emblems/{id} | Soft delete |
| POST | /api/v1/admin/ribbon-emblems/{id}/svg/left | Upload SVG ліва → MinIO `emblems/{id}-left.svg` |
| POST | /api/v1/admin/ribbon-emblems/{id}/svg/right | Upload SVG права → MinIO `emblems/{id}-right.svg` |

### Сповіщення адмінів
| Метод | Шлях | Опис |
|-------|------|------|
| GET | /api/v1/admin/notification-triggers | Список тригерів з конфігом |
| PUT | /api/v1/admin/notification-triggers/{triggerType} | Оновити конфіг тригера |
| GET | /api/v1/admin/notification-triggers/recipients | DB-адміни + Super Admin (id=0) для вибору отримувачів |
| GET | /api/v1/admin/notifications | Мої сповіщення (?limit=50) |
| GET | /api/v1/admin/notifications/unread-count | Кількість непрочитаних |
| POST | /api/v1/admin/notifications/{id}/read | Позначити прочитаним |
| POST | /api/v1/admin/notifications/read-all | Позначити всі прочитаними |

**Тригери:** `new_order`, `order_status_changed`, `order_status_changed:{statusName}` (composite key), `new_user`

**context dict (new_order):** orderNumber, orderUrl, customerName, customerPhone, customerEmail, total, itemCount, deliveryCity, deliveryMethod, paymentMethod, comment

**context dict (order_status_changed):** orderNumber, orderUrl, statusName, previousStatus, customerName, customerPhone, customerEmail, total, deliveryCity, deliveryMethod, adminName

**context dict (new_user):** fullName, userUrl, email, phone, registrationDate

### Аудит-лог
| Метод | Шлях | Опис |
|-------|------|------|
| GET | /api/v1/admin/audit-logs | Журнал дій адмінів (?entityTypes[], entityId, adminId, action, from, to, page, pageSize) |

**TrackedTypes (22):** Order, Product, User, Admin, Role, Delivery, Supplier, ProductCategory, ProductSubcategory, StockProduct, DeliveryMethod, PaymentMethod, OrderStatus, NotificationTriggerConfig, + 6 ribbon types + ConstructorIncompatibility + ConstructorForcedText
**Excluded fields:** PasswordHash, IsDeleted, CreatedAt, UpdatedAt
**adminId=null** → система; **adminId=0** → Super Admin (немає рядка в Admins)

### Конструктор — правила (нові)
| Метод | Шлях | Опис |
|-------|------|------|
| GET | /api/v1/constructor/rules | Публічний — ConstructorRulesResponse (обидва списки) |
| GET | /api/v1/admin/constructor-rules/incompatibilities | Список несумісностей |
| POST | /api/v1/admin/constructor-rules/incompatibilities | Створити |
| PUT | /api/v1/admin/constructor-rules/incompatibilities/{id} | Оновити (Delete+Insert Targets) |
| DELETE | /api/v1/admin/constructor-rules/incompatibilities/{id} | Видалити (CASCADE targets) |
| GET | /api/v1/admin/constructor-rules/forced-texts | Список форс-правил тексту |
| POST | /api/v1/admin/constructor-rules/forced-texts | Створити |
| PUT | /api/v1/admin/constructor-rules/forced-texts/{id} | Оновити (Delete+Insert Values) |
| DELETE | /api/v1/admin/constructor-rules/forced-texts/{id} | Видалити (CASCADE values) |

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

- **AuditInterceptor**: `SaveChangesInterceptor` → `SavingChangesAsync` захоплює зміни до save, `SavedChangesAsync` вставляє AuditLogs після save. Перед 2-м `SaveChangesAsync` — detach всіх non-AuditLog entities щоб уникнути re-save. Використовує `entry.Metadata.ClrType` (не `entry.Entity.GetType()`) — EF Core proxy-типи не кастяться напряму. `ICurrentAdminProvider` — interface в Application, `CurrentAdminProvider` — impl в Api (через `IHttpContextAccessor`). `AddDbContext` через factory pattern: `AddDbContext<AppDbContext>((sp, opts) => opts.UseNpgsql(...).AddInterceptors(sp.GetRequiredService<AuditInterceptor>()))`.
- **Global query filters** на Delivery і Supplier (як на всіх інших). При завантаженні через Include EF Core застосовує фільтр до JOIN — якщо є ризик null, завантажувати окремо з `IgnoreQueryFilters()`.
- **EF Core JSONB**: `OwnsOne(e => e.Field, b => { b.ToJson(); })` — NamesData, RibbonCustomization
- **Soft delete**: `IsDeleted = true`, ніколи не видаляти фізично
- **ProductCategory seed** (ID 1–4 явні через `ValueGeneratedNever()`): ProductCategories — `ValueGeneratedNever()`. ProductSubcategories — `ValueGeneratedOnAdd()` (IDENTITY). Якщо поставити `ValueGeneratedNever()` на Subcategories — EF Core надсилатиме `Id=0` і кожен INSERT конфліктує по PK.
- **StockProduct seed** (ID 1–28 явні): після будь-якого re-seeding потрібна міграція `setval(pg_get_serial_sequence('"StockProducts"', 'Id'), MAX(Id), true)`
- **CurrentStock**: `SUM(income qty) - SUM(outcome qty)` з StockTransactions. Поле `CurrentStock` на `StockVariant` — видалено.
- **Delivery number**: `DEL-{year}-{COUNT+1:D4}`. Unique index на Number захищає від дублів.
- **ReceiveAll** повертає `NoContent()` (204) — не `Ok()`.
- **EF Core projection**: не можна викликати instance методи (наприклад `_imageService.GetPublicUrl()`) всередині LINQ `.Select()` — EF Core кидає `InvalidOperationException: contains a reference to a constant expression through the instance method`. Рішення: спочатку `ToListAsync()`, потім маппінг в пам'яті (`rows.Select(Map)`).
- **Fire-and-forget + DbContext**: scoped `DbContext` не можна шарити між потоками. Будь-яка fire-and-forget операція (email, claim guest orders) мусить отримувати власний DbContext через `IServiceScopeFactory.CreateAsyncScope()`. Інакше — `ObjectDisposedException` або race condition на disposed context.
- **Email activation**: надсилається автоматично при `RegisterAsync` (fire-and-forget). Токен — таблиця `EmailVerificationTokens`, старі токени інвалідуються при повторній відправці. Термін дії 24 год.
- **SVG upload**: `IImageService.UploadAsync(objectKey, stream, "image/svg+xml")`. Емблеми зберігаються під ключами `emblems/{id}-left.svg` / `emblems/{id}-right.svg`.

## Конфігурація

- **БД** (dev): `Host=localhost;Port=5432;Database=vypusknyk_plus;Username=postgres;Password=postgres`
- **JWT**: `Jwt__Key`, 15 хв (user), 8 год (admin)
- **Admin**: `Admin:Email`, `Admin:Password`
- **CORS**: `Cors__AllowedOrigins`
- **MinIO**: `Minio__Endpoint`, `Minio__AccessKey`, `Minio__SecretKey`, `MINIO_PUBLIC_ENDPOINT`
- **Email__AdminPanelUrl**: URL адмін-панелі для посилань у сповіщеннях; у docker-compose = `ADMIN_URL`
- **Google__ClientId**: OAuth 2.0 Client ID (не використовується для верифікації — лише placeholder); env var `GOOGLE_CLIENT_ID` в `.env`
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
