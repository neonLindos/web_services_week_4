using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TeamPractice.Results;
using TeamPractice.Data;
using TeamPractice.Mappings;
using TeamPractice.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressMapClientErrors = true;
    options.InvalidModelStateResponseFactory = context =>
        new BadRequestObjectResult(ReturnResult<object>.Error("Некорректные входные данные."));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<ITeamRepository, TeamRepository>();

var app = builder.Build();

app.UseExceptionHandler(handler => handler.Run(async context =>
{
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(
        ReturnResult<object>.Error("Внутренняя ошибка сервера."));
}));
app.UseStatusCodePages(async context =>
{
    await context.HttpContext.Response.WriteAsJsonAsync(
        ReturnResult<object>.Error($"Ошибка HTTP {context.HttpContext.Response.StatusCode}."));
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
