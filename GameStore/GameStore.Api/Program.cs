using GameStore.Application.Services;
using GameStore.Infrastructure;
using GameStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<GameStoreContext>(options =>
{
    options.UseOracle(builder.Configuration.GetConnectionString("GameStoreOracle"));
});

builder.Services.AddScoped<IGameRepository, GameRepository>();


builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();