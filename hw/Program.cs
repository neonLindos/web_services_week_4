using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hw.Data;
using hw.DTO;
using hw.Mappings;
using hw.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
        new BadRequestObjectResult(new ReturnResult<object>(false, null,
            "Ошибка проверки данных.", context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .ToDictionary(entry => entry.Key, entry => entry.Value!.Errors
                    .Select(error => string.IsNullOrEmpty(error.ErrorMessage)
                        ? "Некорректное значение." : error.ErrorMessage).ToArray())));
    options.SuppressMapClientErrors = true;
});
builder.Services.AddAutoMapper(config => config.AddProfile<MappingProfile>());
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IBookRepository, BookRepository>();

var app = builder.Build();
app.UseExceptionHandler(handler => handler.Run(async context =>
{
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(
        new ReturnResult<object>(false, null, "Внутренняя ошибка сервера."));
}));
app.UseStatusCodePages(async context =>
{
    await context.HttpContext.Response.WriteAsJsonAsync(
        new ReturnResult<object>(false, null,
            $"Ошибка HTTP {context.HttpContext.Response.StatusCode}."));
});
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Book Catalog API"));
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
