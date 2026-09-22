using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ProductLab.Results;
using ProductLab.Data;
using ProductLab.Mappings;
using ProductLab.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressMapClientErrors = true;
    options.InvalidModelStateResponseFactory = context =>
        new BadRequestObjectResult(ReturnResult<object>.Failure("Некорректные входные данные."));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(_ => { }, typeof(AutoMapperProfile));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

app.UseExceptionHandler(handler => handler.Run(async context =>
{
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(
        ReturnResult<object>.Failure("Внутренняя ошибка сервера."));
}));
app.UseStatusCodePages(async context =>
{
    await context.HttpContext.Response.WriteAsJsonAsync(
        ReturnResult<object>.Failure($"Ошибка HTTP {context.HttpContext.Response.StatusCode}."));
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
