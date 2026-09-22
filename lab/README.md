# Лабораторная работа — Модуль 04

**Тема:** EF Core, Repository, DTO и AutoMapper.
**Проект:** ProductLab (Product).
**Используемые технологии:** .NET 10, ASP.NET Core, SQLite, AutoMapper, Swagger.

Условие: [Markdown-копия](Модуль_04_Лабораторная_работа.md), [оригинал](../Модуль%2004%20Лабораторная%20работа.docx).

## Как запустить

В этой папке выполните:

```powershell
dotnet tool restore
dotnet restore
dotnet ef database update
dotnet run --launch-profile http
```

Откройте http://localhost:5263/swagger. Строка подключения — в appsettings.json, SQLite создаётся миграцией Code First. Отдельный сервер БД не нужен.

## Что реализовано

| Метод | Endpoint | Назначение | Успех |
|---|---|---|---|
| GET | `/api/products` | Все записи | 200 |
| GET | `/api/products/{id}` | Одна запись | 200 |
| POST | `/api/products` | Создать | 201 + Location |
| PUT | `/api/products/{id}` | Изменить | 200 |
| DELETE | `/api/products/{id}` | Удалить | 200 |

Отсутствующий id возвращает 404. Ошибки привязки JSON возвращают 400. Ответы используют ReturnResult, включая ошибки; HTTP-код сохраняет смысл результата.

DTO содержит только Id, Name, Price. Поле Category есть в Entity, но не передаётся через DTO. При создании оно остаётся пустым; при изменении существующее значение сохраняется. Id для POST назначает БД, для PUT используется id из маршрута.

## Демонстрация

В Swagger выполните POST с телом:

```json
{"id":0,"name":"Ноутбук","price":350000}
```

Сохраните возвращённый id. Выполните GET по нему, PUT с изменёнными полями, DELETE и повторный GET (404). Примеры запросов — в [ProductLab.http](ProductLab.http); подставьте фактический id вместо 1.

## Контрольные вопросы

1. **Entity и DTO** — Entity описывает таблицу, DTO ограничивает данные, которыми обменивается клиент.
2. **DbContext и DbSet** — контекст управляет запросами и изменениями, DbSet представляет набор сущностей.
3. **Code First** — схема создаётся из модели с помощью миграций.
4. **Repository** — скрывает EF Core за интерфейсом; контроллер не обращается к контексту напрямую.
5. **AutoMapper** — выполняет преобразование Entity ↔ DTO через CreateMap и ReverseMap.
6. **ReturnResult** — задаёт общий формат тела ответа, сохраняя правильные HTTP-коды.

## Результат работы

| Требование | Реализация |
|---|---|
| БД и конфигурация | AppDbContext, appsettings.json, Migrations |
| Доступ через Repository | IProductRepository, ProductRepository |
| DTO и AutoMapper | ProductDto, профиль маппинга |
| CRUD | Контроллер |
| Единый контракт | ReturnResult, обработка ошибок в Program.cs |

Автоматическая проверка из корня репозитория: `dotnet run --project checks`.
Она проверяет миграции на SQLite в памяти, CRUD, 404, маппинг и сохранение скрытого поля. Веб-сервер при проверке не запускается; демонстрация Swagger выполняется отдельно.
