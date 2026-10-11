# CSE5032 · Модуль 04 — EF Core, Repository, DTO и AutoMapper

**Работу выполнил:** Рябинин Максим

Четвёртая тема курса **«Разработка веб-сервисов»**: работа с базой данных через Entity Framework Core, миграции Code First, выделение Repository, преобразование Entity ↔ DTO через AutoMapper и единый формат ответа ReturnResult.

## Все работы по курсу CSE5032

| Неделя | Тема | Репозиторий |
|---|---|---|
| 1 | Введение в ASP.NET Core | [web_services_week_1](https://github.com/neonLindos/web_services_week_1) |
| 2 | Web API + CRUD | [web_services_week_2](https://github.com/neonLindos/web_services_week_2) |
| 3 | Dependency Injection и логирование | [web_services_week_3](https://github.com/neonLindos/web_services_week_3) |
| 4 | EF Core, Repository, DTO и AutoMapper | **этот репозиторий** |

| Работа | Проект | Ресурс | Что показано |
|--------|--------|--------|-----------|
| [hw/](hw/) | `hw` | Book, Author | полный CRUD `/api/books`, поиск по автору, валидация DTO, ReturnResult |
| [lab/](lab/) | `ProductLab` | Product | полный CRUD `/api/products`, ProductDto без Category, Repository, AutoMapper |
| [prac/](prac/) | `TeamPractice` | Team | полный CRUD `/api/team`, поиск по городу, TeamDto без Description |

## Как запустить

Нужен [.NET 10 SDK](https://dotnet.microsoft.com/download). Из корня репозитория:

```powershell
cd hw   # или lab, prac
dotnet run
```

Миграции применяются автоматически при запуске, SQLite-файл создаётся сам.

Откройте Swagger UI:

| Проект | Адрес |
|---|---|
| hw | http://localhost:5294/swagger |
| lab | http://localhost:5263/swagger |
| prac | http://localhost:5191/swagger |

## Структура

Каждая подпапка `hw/`, `lab/`, `prac/` — самостоятельный проект:

- `.docx` в корне — методичка от преподавателя, как выдана;
- `Модуль_04_*.md` — та же методичка в Markdown;
- `README.md` — решение: что реализовано, ответы на контрольные вопросы, итоговая таблица;
- `Migrations/` — миграции для создания и обновления базы данных;
- `.http` — примеры запросов для демонстрации API.

Данные во всех решениях хранятся в SQLite. У каждого проекта своя база и строка подключения в `appsettings.json`; база создаётся автоматически при запуске: `Program.cs` вызывает `Database.Migrate()`. Локальные базы и результаты сборки исключены через `.gitignore`.

## Проверка

Из корня репозитория:

```powershell
dotnet run --project checks
```

Проверки используют SQLite в памяти: миграции, CRUD, AutoMapper, ответы 404, поиск автора и города, скрытие полей DTO и их сохранение при PUT. Для книг также проверяются валидация, повторное использование автора и обновление старой схемы. Веб-сервер не запускается; сценарии демонстрации через Swagger приведены в README проектов.

## Адреса и эндпоинты

| Работа | Swagger | GET списка (JSON) |
|---|---|---|
| Домашняя | http://localhost:5294/swagger | http://localhost:5294/api/books |
| Лабораторная | http://localhost:5263/swagger | http://localhost:5263/api/products |
| Практическая | http://localhost:5191/swagger | http://localhost:5191/api/team |

На главной странице `/` интерфейса нет; для демонстрации открывайте `/swagger`.
Запуск — `make` в папке `hw`, `lab` или `prac`.
Все пути ниже относятся к адресу соответствующей работы. `{id}` — числовой ID.

| Метод | Домашняя | Лабораторная | Практическая | Назначение |
|---|---|---|---|---|
| GET | `/api/books` | `/api/products` | `/api/team` | Получить список |
| GET | `/api/books/{id}` | `/api/products/{id}` | `/api/team/{id}` | Получить запись по ID |
| POST | `/api/books` | `/api/products` | `/api/team` | Создать запись |
| PUT | `/api/books/{id}` | `/api/products/{id}` | `/api/team/{id}` | Изменить запись |
| DELETE | `/api/books/{id}` | `/api/products/{id}` | `/api/team/{id}` | Удалить запись |

Дополнительный поиск:

- `GET /api/books?author=Булгаков` — книги по части имени автора.
- `GET /api/team/city/{city}` — команды указанного города; замените `{city}` названием города.

GET, PUT и DELETE при успехе возвращают 200; POST — 201 с Location.
Отсутствующая запись при GET по ID, PUT или DELETE возвращает 404.
Ответы используют оболочку `ReturnResult`; примеры JSON и требования к входным
данным приведены в README каждой работы. POST, PUT и DELETE выполняйте через Swagger.
