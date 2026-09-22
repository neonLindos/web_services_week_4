using AutoMapper;
using hw.Controllers;
using hw.Data;
using hw.DTO;
using hw.Mappings;
using hw.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging.Abstractions;
using System.ComponentModel.DataAnnotations;

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();
var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
await using var context = new AppDbContext(options);
await context.GetService<IMigrator>().MigrateAsync("20260922071238_InitialCreate");
await context.Database.ExecuteSqlRawAsync("INSERT INTO Autors (Id, Name) VALUES (1, 'Old author')");
await context.Database.ExecuteSqlRawAsync("INSERT INTO Books (Title, AutorId, AuthorId, Price, Description) VALUES ('Old book', 123, 1, '10', 'Original')");
await context.Database.MigrateAsync();
var old = await context.Books.SingleAsync();
Check(old.Year == 0 && old.Title == "Old book" && old.AuthorId == 1, "Миграция исказила старую книгу.");
context.Books.Remove(old);
await context.SaveChangesAsync();

var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance);
config.AssertConfigurationIsValid();
var controller = new BookController(new BookRepository(context), config.CreateMapper());
var input = new SaveBookDto { Title = "Книга", AuthorName = "Автор", Year = 2020, Price = 100, Description = "Описание" };
var created = (CreatedAtActionResult)(await controller.Create(input)).Result!;
var book = ((ReturnResult<BookDto>)created.Value!).Data!;
Check(created.StatusCode == 201 && book.Id > 0 && book.AuthorName == "Автор" && book.Year == 2020 && book.Description == "Описание", "Ошибка POST/маппинга.");
Check(created.ActionName == nameof(BookController.GetById), "Неверный Location.");
context.ChangeTracker.Clear();
var found = (OkObjectResult)(await controller.GetById(book.Id)).Result!;
Check(((ReturnResult<BookDto>)found.Value!).Data!.AuthorName == "Автор", "Автор не загружается.");
input.AuthorName = "Другой автор";
input.Year = 2024;
input.Price = 200;
var updated = (OkObjectResult)(await controller.Update(book.Id, input)).Result!;
Check(((ReturnResult<BookDto>)updated.Value!).Data!.AuthorName == input.AuthorName, "Ошибка PUT.");
context.ChangeTracker.Clear();
var search = (OkObjectResult)(await controller.GetAll("Другой")).Result!;
Check(((ReturnResult<IEnumerable<BookDto>>)search.Value!).Data!.Single().Year == 2024, "Ошибка поиска/сохранения PUT.");
var empty = (OkObjectResult)(await controller.GetAll("Нет такого автора")).Result!;
Check(!((ReturnResult<IEnumerable<BookDto>>)empty.Value!).Data!.Any(), "Поиск должен вернуть пустой список.");
await controller.Create(input);
Check(await context.Autors.CountAsync(a => a.Name == input.AuthorName) == 1, "Дублирование автора.");
Check((await controller.Delete(book.Id)).Result is OkObjectResult, "Ошибка DELETE.");
Check((await controller.GetById(book.Id)).Result is NotFoundObjectResult, "GET должен вернуть 404.");
Check((await controller.Update(book.Id, input)).Result is NotFoundObjectResult, "PUT должен вернуть 404.");
Check((await controller.Delete(book.Id)).Result is NotFoundObjectResult, "DELETE должен вернуть 404.");
var errors = new List<ValidationResult>();
var invalid = new SaveBookDto { Title = " ", AuthorName = "", Year = 0, Price = -1 };
Check(!Validator.TryValidateObject(invalid, new ValidationContext(invalid), errors, true), "Валидация пропускает ошибки.");
Console.WriteLine("PASS: миграция с сохранением данных, AutoMapper, CRUD, поиск, повторное использование автора, 404 и валидация DTO.");


using (var productConnection = new SqliteConnection("Data Source=:memory:"))
{
    await productConnection.OpenAsync();
    var productOptions = new DbContextOptionsBuilder<ProductLab.Data.AppDbContext>().UseSqlite(productConnection).Options;
    await using var db = new ProductLab.Data.AppDbContext(productOptions);
    await db.Database.MigrateAsync();
    var mapping = new MapperConfiguration(cfg => cfg.AddProfile<ProductLab.Mappings.AutoMapperProfile>(), NullLoggerFactory.Instance);
    mapping.AssertConfigurationIsValid();
    var api = new ProductLab.Controllers.ProductsController(new ProductLab.Repositories.ProductRepository(db), mapping.CreateMapper());
    var dto = new ProductLab.DTO.ProductDto { Id = 999, Name = "Ноутбук", Price = 350000 };
    var response = (CreatedAtActionResult)(await api.Create(dto)).Result!;
    var product = ((ProductLab.Results.ReturnResult<ProductLab.DTO.ProductDto>)response.Value!).Result!;
    Check(product.Id != 999 && response.StatusCode == 201, "Product POST/Id.");
    db.ChangeTracker.Clear();
    await db.Database.ExecuteSqlRawAsync("UPDATE Products SET Category = 'Electronics'");
    Check((await api.GetById(product.Id)).Result is OkObjectResult, "Product GET.");
    dto.Name = "Updated";
    await api.Update(product.Id, dto);
    db.ChangeTracker.Clear();
    Check((await db.Products.SingleAsync()).Category == "Electronics", "PUT потерял Category.");
    Check((await db.Products.SingleAsync()).Name == "Updated", "Product PUT.");
    db.ChangeTracker.Clear();
    var all = (OkObjectResult)(await api.GetAll()).Result!;
    Check(!System.Text.Json.JsonSerializer.Serialize(all.Value).Contains("Category"), "Category попала в DTO.");
    await api.Delete(product.Id);
    Check((await api.GetById(product.Id)).Result is NotFoundObjectResult, "Product GET 404.");
    Check((await api.Update(product.Id, dto)).Result is NotFoundObjectResult, "Product PUT 404.");
    Check((await api.Delete(product.Id)).Result is NotFoundObjectResult, "Product DELETE 404.");
}

using (var teamConnection = new SqliteConnection("Data Source=:memory:"))
{
    await teamConnection.OpenAsync();
    var teamOptions = new DbContextOptionsBuilder<TeamPractice.Data.AppDbContext>().UseSqlite(teamConnection).Options;
    await using var db = new TeamPractice.Data.AppDbContext(teamOptions);
    await db.Database.MigrateAsync();
    var mapping = new MapperConfiguration(cfg => cfg.AddProfile<TeamPractice.Mappings.MappingProfile>(), NullLoggerFactory.Instance);
    mapping.AssertConfigurationIsValid();
    var api = new TeamPractice.Controllers.TeamController(new TeamPractice.Repositories.TeamRepository(db), mapping.CreateMapper());
    var dto = new TeamPractice.DTO.TeamDto { Id = 999, Name = "Team", City = "Astana" };
    var response = (CreatedAtActionResult)(await api.Create(dto)).Result!;
    var team = ((TeamPractice.Results.ReturnResult<TeamPractice.DTO.TeamDto>)response.Value!).Data!;
    Check(team.Id != 999 && response.StatusCode == 201, "Team POST/Id.");
    db.ChangeTracker.Clear();
    await db.Database.ExecuteSqlRawAsync("UPDATE Teams SET Description = 'Internal'");
    dto.City = "Almaty";
    await api.Update(team.Id, dto);
    db.ChangeTracker.Clear();
    Check((await db.Teams.SingleAsync()).Description == "Internal", "PUT потерял Description.");
    db.ChangeTracker.Clear();
    await api.Create(new TeamPractice.DTO.TeamDto { Name = "Other", City = "Astana" });
    db.ChangeTracker.Clear();
    var citySearch = (OkObjectResult)(await api.GetByCity("Almaty")).Result!;
    Check(((TeamPractice.Results.ReturnResult<IEnumerable<TeamPractice.DTO.TeamDto>>)citySearch.Value!).Data!.Single().Id == team.Id, "Team поиск по городу.");
    var noMatch = (OkObjectResult)(await api.GetByCity("Alm")).Result!;
    Check(!((TeamPractice.Results.ReturnResult<IEnumerable<TeamPractice.DTO.TeamDto>>)noMatch.Value!).Data!.Any(), "Город должен совпадать полностью.");
    var all = (OkObjectResult)(await api.GetAll()).Result!;
    Check(!System.Text.Json.JsonSerializer.Serialize(all.Value).Contains("Description"), "Description попала в DTO.");
    Check((await api.GetById(team.Id)).Result is OkObjectResult, "Team GET.");
    await api.Delete(team.Id);
    Check((await api.GetById(team.Id)).Result is NotFoundObjectResult, "Team GET 404.");
    Check((await api.Update(team.Id, dto)).Result is NotFoundObjectResult, "Team PUT 404.");
    Check((await api.Delete(team.Id)).Result is NotFoundObjectResult, "Team DELETE 404.");
}
Console.WriteLine("PASS: Product и Team — миграции, AutoMapper, CRUD, 404, скрытые поля DTO и точный поиск города.");
