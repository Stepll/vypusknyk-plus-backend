# Випускник+ (VypusknykPlus) Backend

## Опис проекту

E-commerce платформа для замовлення персоналізованих випускних стрічок, медалей, грамот та аксесуарів для українських шкіл. Бекенд обслуговує React-фронтенд (папка `../Test_project`).

## Технології

- **.NET 8.0**, ASP.NET Core Web API, C#
- **PostgreSQL** + Entity Framework Core 8 (Npgsql)
- **JWT Bearer** автентифікація (BCrypt для хешування паролів)
- **Swagger/OpenAPI** для документації API
- Layered architecture: `Api` (контролери, middleware) + `Application` (сервіси, сутності, дані)

## Структура проекту

```
src/
├── VypusknykPlus.Api/              # Presentation layer
│   ├── Controllers/                # AuthController, ProductsController, OrdersController, DesignsController
│   ├── Middleware/                  # ExceptionHandlingMiddleware
│   └── Program.cs                  # DI, CORS, JWT, pipeline
│
└── VypusknykPlus.Application/      # Business logic layer
    ├── Entities/                   # User, Product, Order, OrderItem, SavedDesign, CartItem (BaseEntity з soft delete)
    ├── ValueObjects/               # DeliveryInfo, RecipientInfo, RibbonState, RibbonCustomization, ProductSnapshot, NamesData
    ├── DTOs/                       # Auth/, Orders/, Products/, Designs/
    ├── Data/
    │   ├── AppDbContext.cs         # DbContext з global query filters (soft delete)
    │   ├── Configurations/         # Fluent API конфігурації для кожної сутності
    │   └── Migrations/             # InitialCreate (2026-04-07)
    └── Services/                   # AuthService, ProductService, OrderService, DesignService + інтерфейси
```

## API ендпоінти

| Метод  | Шлях                      | Опис                       | Авторизація |
|--------|---------------------------|----------------------------|-------------|
| POST   | /api/v1/auth/login        | Логін                      | -           |
| POST   | /api/v1/auth/register     | Реєстрація                 | -           |
| POST   | /api/v1/auth/refresh      | Оновити токен              | -           |
| POST   | /api/v1/auth/forgot-password | Запит на відновлення     | -           |
| POST   | /api/v1/auth/reset-password  | Скинути пароль           | -           |
| PUT    | /api/v1/auth/profile      | Оновити профіль            | JWT         |
| PUT    | /api/v1/auth/password     | Змінити пароль              | JWT         |
| GET    | /api/v1/products          | Список продуктів (фільтри) | -           |
| GET    | /api/v1/products/{id}     | Деталі продукту            | -           |
| POST   | /api/v1/products/{id}/image | Завантажити зображення   | JWT         |
| POST   | /api/v1/orders            | Створити замовлення        | JWT         |
| GET    | /api/v1/orders            | Замовлення користувача     | JWT         |
| GET    | /api/v1/orders/{id}       | Деталі замовлення          | JWT         |
| POST   | /api/v1/designs           | Зберегти дизайн            | JWT         |
| GET    | /api/v1/designs           | Дизайни користувача        | JWT         |
| PUT    | /api/v1/designs/{id}      | Оновити дизайн             | JWT         |
| DELETE | /api/v1/designs/{id}      | Видалити дизайн            | JWT         |
| GET    | /api/v1/cart              | Кошик користувача          | JWT         |
| POST   | /api/v1/cart              | Додати товар в кошик       | JWT         |
| PATCH  | /api/v1/cart/{id}         | Оновити кількість          | JWT         |
| DELETE | /api/v1/cart/{id}         | Видалити з кошика          | JWT         |
| DELETE | /api/v1/cart              | Очистити кошик             | JWT         |

## Конфігурація

- **БД**: PostgreSQL на `localhost:5432`, база `vypusknyk_plus` (user: postgres/postgres)
- **JWT**: ключ з env var `Jwt__Key` (dev: `appsettings.Development.json`), expiration 15 хв
- **CORS**: origins з `Cors__AllowedOrigins` env var або `appsettings.json`
- **MinIO**: S3-compatible object storage, bucket `products` (public read), env vars `Minio__*`
- **Порт**: HTTP `localhost:5272` (dev), `8080` (Docker)

## Команди

```bash
# Запуск (dev)
dotnet run --project src/VypusknykPlus.Api

# Міграції (використовує AppDbContextFactory — не потребує запущеного MinIO/DB)
dotnet ef migrations add <Name> --project src/VypusknykPlus.Application --startup-project src/VypusknykPlus.Api
dotnet ef database update --project src/VypusknykPlus.Application --startup-project src/VypusknykPlus.Api

# Docker
cp .env.example .env   # заповнити секрети
docker compose up --build
```

## Фронтенд (../Test_project)

- React 19 + TypeScript + Vite
- MobX (стейт), Ant Design + TailwindCSS (UI), Framer Motion (анімації)
- API base URL: `http://localhost:5272` (env var `VITE_API_URL`)
- 15 сторінок: Home, Catalog, ProductPage, Cart, Checkout, RibbonConstructor, ConstructorHub, Auth, Account, OrderDetail, OrderSuccess, Events, About, Contacts, NotFound
- MobX stores: CartStore (кошик), AuthStore (авторизація + дизайни), ToastStore (сповіщення)

## Статус реалізації

### Готово
- [x] Автентифікація (login, register, profile update, password change)
- [x] JWT токени + BCrypt хешування
- [x] Сутності та міграції БД (User, Product, Order, OrderItem, SavedDesign, CartItem)
- [x] Value objects з JSONB зберіганням (RibbonState, NamesData, DeliveryInfo, etc.)
- [x] Global query filters (soft delete)
- [x] ExceptionHandlingMiddleware
- [x] Swagger документація
- [x] CORS налаштування
- [x] Контролери з маршрутизацією для всіх ресурсів
- [x] Serilog (Console + File sinks, request logging, structured logging)
- [x] FluentValidation (автоматична валідація через ActionFilter, валідатори для всіх request DTOs)
- [x] Seed data (15 продуктів з фронтенду, міграція SeedProducts)
- [x] ProductService + Pagination (фільтрація, сортування, пошук, PagedResponse)
- [x] DesignService (CRUD збережених дизайнів стрічок)
- [x] CartController + CartService (add, get, update qty, remove, clear)
- [x] OrderService (create з генерацією OrderNumber, get user orders, get by id)
- [x] JWT Refresh Tokens (30 днів, rotation, revoke при зміні пароля)
- [x] Email сервіс (MailKit, SMTP) + Forgot/Reset Password (token 1 год, revoke refresh tokens)

### TODO — Рекомендований порядок виконання

Порядок підібраний щоб мінімізувати переробки. Критичні пари що **не можна розривати**: `ProductService + Pagination`, `Email + ForgotPassword`.

---

#### Фаза 1 — Фундамент (до будь-якої бізнес-логіки)

- [x] **Логування (Serilog)** — Console + File sinks, request logging, конфігурація через appsettings.json
- [x] **Валідація DTO (FluentValidation)** — ValidationActionFilter + валідатори для Auth, Orders, Designs
- [x] **Seed data** — 15 продуктів з фронтенду перенесено через EF Core HasData + міграція SeedProducts

---

#### Фаза 2 — Основні фічі (порядок важливий через залежності)

- [x] **ProductService + Pagination** — `GetAllAsync()` з фільтрацією (category, sort, search), `GetByIdAsync()`, `PagedResponse<T>` з `{ items, total, page, pageSize }`
- [x] **DesignService** — `SaveAsync()`, `GetUserDesignsAsync()`, `UpdateAsync()`, `DeleteAsync()` з soft delete
- [x] **Cart endpoints** — `CartController` + `CartService`: add, get, update qty, remove, clear; snapshot продукту при додаванні
- [x] **OrderService** — `CreateAsync()` (генерація OrderNumber "VIP-XXXXXX", розрахунок total, маппінг delivery/payment), `GetUserOrdersAsync()`, `GetByIdAsync()`

---

#### Фаза 3 — Розширення Auth

- [x] **JWT refresh tokens** — access 15 хв + refresh 30 днів, token rotation, revoke all on password change, міграція AddRefreshTokens
- [x] **Email сервіс + Forgot password** — MailKit SMTP, forgot-password (токен 1 год), reset-password, email enumeration protection, order confirmation email готовий

---

#### Фаза 4 — Інфраструктура

- [x] **Production конфігурація** — всі секрети через env vars, startup validation, CORS з конфігу, bootstrap logger читає env vars
- [x] **Product images (MinIO)** — `IImageService` + `ImageService`, bucket auto-init при старті, `POST /api/v1/products/{id}/image`, `Product.ImageKey`, `ProductResponse.ImageUrl`, міграція `AddProductImageKey`
- [x] **Docker** — multi-stage `Dockerfile`, `docker-compose.yml` (api + db + minio), `.dockerignore`, `.env.example`, `IDesignTimeDbContextFactory`, health checks `/healthz`, auto-migrations при старті
- [x] **Тести** — xUnit unit тести для AuthService (8 тестів), EF InMemory + Moq, проект `VypusknykPlus.Tests`
- [x] **CI/CD** — GitHub Actions: CI (build+test на кожен push), Deploy (push до main → Docker Hub → SSH на сервер Contabo)

---

#### Фаза 5 — Запуск продукту

- [x] **Підключити фронт до бека** — `VITE_API_URL=https://75.119.152.4.sslip.io` в Vercel env vars; API шар `src/api/{types,products,orders,designs,token}.ts`; Catalog, ProductPage, Checkout, Account, OrderDetail підключені до реального API; CartStore персистується в localStorage
- [x] **Задеплоїти фронт на Vercel** — `https://vypusknyk-plus-fronend.vercel.app`, автодеплой при push до main; `vercel.json` SPA rewrites
- [x] **HTTPS без домену** — Nginx reverse proxy + Let's Encrypt для `75.119.152.4.sslip.io`; бек доступний по HTTPS
- [ ] **Налаштувати email** — додати SMTP дані в `.env` на сервері (`~/vypusknyk-plus/prod/.env`)
- [ ] **Домен** — купити домен і прив'язати до сервера (75.119.152.4); оновити Nginx + Let's Encrypt
- [ ] **Адмінка** — панель для управління замовленнями та продуктами

---

#### Відомі обмеження / технічний борг

- Кошик не синхронізується з беком (тільки localStorage) — гость і авторизований користувач мають окремі кошики
- Constructor ribbons (`productId = 0`) не можуть бути оформлені через `/api/v1/orders` (валідатор вимагає `productId > 0`)
- JWT refresh token не використовується на фронті (тільки access token; при закінченні сесії — logout)
- Зображення продуктів (MinIO) не завантажені — каталог показує кольорові заглушки

---

#### Фаза 6 — Критично для бізнесу

- [ ] **Конструктор → замовлення** — додати продукт "Кастомна стрічка" в БД (або дозволити `productId = null` на беці), щоб constructor ribbons можна було оформити через `/api/v1/orders`
- [ ] **JWT refresh на фронті** — автоматично оновлювати access token через `/api/v1/auth/refresh` замість logout при закінченні сесії (15 хв)
- [ ] **Email (SMTP)** — заповнити `EMAIL_SMTP_HOST`, `EMAIL_SMTP_USER`, `EMAIL_SMTP_PASSWORD`, `EMAIL_FROM_EMAIL` у `.env` на сервері; підтвердження замовлення надсилатиметься автоматично
- [ ] **Зображення продуктів** — завантажити реальні фото через `POST /api/v1/products/{id}/image` (MinIO); оновити `MINIO_PUBLIC_ENDPOINT` в `.env`

---

#### Фаза 7 — Зростання

- [ ] **Домен** — купити домен, прив'язати до `75.119.152.4`; оновити Nginx + Let's Encrypt (`certbot --nginx -d yourdomain.com`); замінити `sslip.io` у `VITE_API_URL` на Vercel
- [ ] **Адмінка** — панель для перегляду/зміни статусу замовлень, управління продуктами (новий розділ або окремий React-додаток з роллю `Admin`)
