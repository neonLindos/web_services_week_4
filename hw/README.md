# Домашняя работа — Модуль 04

**Тема:** DTO, AutoMapper, Repository и EF Core — каталог книг.
**Проект:** `hw` (Book, Author).
**Используемые технологии:** существующий .NET 10, ASP.NET Core, EF Core SQLite, AutoMapper, Swagger UI.

Условие: [Markdown-копия](Модуль_04_Домашнее_задание.md), [оригинал](../Модуль%2004%20Домашнее%20задание.docx).

## Как запустить

1. Откройте `hw.csproj` в Visual Studio или терминал в этой папке. Нужен .NET 10 SDK.
2. Восстановите зависимости и примените миграции:

   ```powershell
   dotnet restore
   dotnet ef database update
   ```

   Используется установленный инструмент `dotnet-ef` версии 10. Строка подключения находится в `appsettings.json`: `Data Source=database.db`. SQLite работает как файловая библиотека, отдельный сервер БД не нужен.
3. Запустите приложение:

   ```powershell
   dotnet run --launch-profile http
   ```

4. Откройте <http://localhost:5294/swagger>. Swagger доступен в Development, описание API — `/openapi/v1.json`.

Миграция `CompleteBookCatalog` удаляет ошибочный столбец `AutorId` и добавляет `Year`, сохраняя книги и действующую связь `AuthorId`. Для старых записей год неизвестен и равен `0`: укажите настоящий год через PUT. Исходная миграция сохранена. Рабочая база при проверке решения не изменялась.

## Что реализовано

| Метод | Endpoint | Назначение | Успех | Ошибка |
|---|---|---|---|---|
| GET | `/api/books` | Все книги | 200 | — |
| GET | `/api/books?author=Булгаков` | Поиск по части имени автора | 200 | — |
| GET | `/api/books/{id}` | Одна книга | 200 | 404 |
| POST | `/api/books` | Создать книгу | 201 + Location | 400 |
| PUT | `/api/books/{id}` | Изменить все поля книги | 200 | 400, 404 |
| DELETE | `/api/books/{id}` | Удалить книгу | 200 | 404 |

Поиск использует `Contains` SQLite и учитывает регистр; пустой параметр возвращает все книги, отсутствие совпадений — пустой список. Автор создаётся по имени автоматически или используется существующий с таким же именем после удаления пробелов по краям. При удалении книги автор сохраняется.

Клиент отправляет `SaveBookDto`, получает `BookDto`. Навигационные свойства и внутренний `AuthorId` не выдаются, циклических ссылок в JSON нет. AutoMapper преобразует DTO в Entity при создании/изменении и Entity в DTO при чтении. Репозиторий загружает автора через `Include`.

Ограничения входных данных: обязательные Title (до 200 символов), AuthorName (до 150), Year от 1 до 9999, Price от 0 до 1 000 000 000, Description до 4000 символов. Id назначает БД.

Все ответы имеют оболочку `ReturnResult<T>`:

```json
{
  "success": true,
  "data": {
    "id": 1,
    "title": "Мастер и Маргарита",
    "authorName": "Михаил Булгаков",
    "price": 1500,
    "year": 1967,
    "description": "Роман"
  },
  "message": null,
  "errors": null
}
```

Ошибка содержит `success: false`, `data: null` и сообщение; ошибки валидации дополнительно содержат словарь `errors`. Удаление возвращает 200 с сообщением, чтобы сохранить единый формат тела. Необработанные исключения возвращают 500 без внутренних подробностей.

## Демонстрация через Swagger

1. POST: отправьте пример из [hw.http](hw.http), запомните `data.id` и проверьте Location.
2. GET по этому id: проверьте поля книги и имя автора.
3. PUT: измените цену и описание, затем повторите GET.
4. GET `/api/books?author=Булгаков`: проверьте результат поиска.
5. DELETE по id, затем GET по тому же id: ожидается 404.
6. POST с пустым названием, годом 0 и отрицательной ценой: ожидается 400 в формате ReturnResult.

## Контрольные вопросы

1. **Entity** — класс, который EF Core связывает с таблицей БД. Book хранит книгу, Author — автора, AuthorId задаёт связь.
2. **DTO** — контракт обмена с клиентом. SaveBookDto ограничивает ввод, BookDto содержит только поля ответа.
3. **DbContext** — сеанс работы с БД: запросы, отслеживание изменений и сохранение. Схема создаётся миграциями Code First.
4. **Repository** — отделяет контроллер от запросов EF Core. Контроллер вызывает IBookRepository, реализация работает с AppDbContext.
5. **AutoMapper** — преобразует объекты по MappingProfile, включая Author.Name → AuthorName. Связь с существующим автором выбирает репозиторий.
6. **ReturnResult** — единая оболочка результата, данных, сообщения и ошибок. HTTP-код по-прежнему отражает успех или причину отказа.

## Проверка

```powershell
dotnet build
dotnet run --project ../checks/checks.csproj
dotnet ef migrations has-pending-model-changes
```

Консольная проверка использует SQLite в памяти, не запускает веб-сервер и не меняет `database.db`. Проверяет обновление старой схемы с сохранением книги, конфигурацию AutoMapper, CRUD, поиск, повторное использование автора, 404 и валидацию DTO. HTTP-конвейер и интерфейс Swagger нужно проверить после самостоятельного запуска по сценарию выше.

Подключение Swagger UI к встроенному OpenAPI соответствует [документации ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-10.0). Регистрация AutoMapper использует обязательный конфигурационный callback — см. [руководство AutoMapper](https://docs.automapper.io/en/stable/15.0-Upgrade-Guide.html).

## Результат работы

| Компонент задания | Реализация |
|---|---|
| EF Core, Code First, конфигурация | AppDbContext, миграции, appsettings.json |
| Repository | IBookRepository, BookRepository |
| DTO и AutoMapper | BookDto, SaveBookDto, MappingProfile |
| Единый формат | ReturnResult, настройка ошибок в Program.cs |
| CRUD и поиск | BookController |
| Демонстрация | Swagger UI и hw.http |
