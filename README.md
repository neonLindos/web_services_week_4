# CSE5032 · Модуль 04 — EF Core, Repository, DTO и AutoMapper

**Работу выполнил:** Рябинин Максим.

Три самостоятельных проекта на .NET 10 / ASP.NET Core с SQLite и миграциями Code First.

| Работа | Проект | Требования из DOCX |
|---|---|---|
| [Домашняя](hw/README.md) | Каталог книг | CRUD, поиск автора, Year, Price, Description, DTO, AutoMapper, ReturnResult |
| [Лабораторная](lab/README.md) | Товары | CRUD, ProductDto без Category, Repository, AutoMapper, ReturnResult |
| [Практическая](prac/README.md) | Команды | CRUD, поиск города, TeamDto без Description, Repository, AutoMapper, ReturnResult |

Оригиналы трёх заданий сохранены в корне в DOCX. Markdown-копии находятся рядом с решениями. Название репозитория продолжает принятую нумерацию; содержание сверено именно с заданиями модуля 04.

## Запуск

Нужен .NET 10 SDK. Из корня репозитория:

```powershell
dotnet tool restore
cd hw # либо lab или prac
dotnet restore
dotnet ef database update
dotnet run --launch-profile http
```

| Проект | Swagger |
|---|---|
| hw | http://localhost:5294/swagger |
| lab | http://localhost:5263/swagger |
| prac | http://localhost:5191/swagger |

Каждый проект использует свою SQLite-базу и строку подключения в appsettings.json. Базы не хранятся в Git: они создаются миграциями. API запускаются пользователем.

## Проверка соответствия

```powershell
dotnet run --project checks
```

Консольные проверки выполняются без веб-сервера на SQLite в памяти: миграции, CRUD всех трёх API, AutoMapper, ответы 404, поиск автора и города, исключение Category/Description из DTO и сохранение этих полей при PUT. Для книг также проверяются валидация DTO, повторное использование автора и обновление старой схемы без потери книг.

Проверки не заменяют демонстрацию HTTP и Swagger: последовательности запросов приведены в README проектов и файлах .http. Локальные рабочие базы при проверке не изменяются.

## Состав репозитория

Исходники, проекты, миграции, конфигурация без секретов, HTTP-примеры, проверки и задания включены. Сборки bin/obj, локальные базы, настройки IDE, временные файлы Office и repomix-output исключены через .gitignore.

## Другие работы

- [Модуль 01](https://github.com/neonLindos/web_services_week_1)
- [Модуль 02](https://github.com/neonLindos/web_services_week_2)
- [Модуль 03](https://github.com/neonLindos/web_services_week_3)
