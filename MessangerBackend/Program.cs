using MessangerBackend.Core.Interfaces;
using MessangerBackend.Core.Services;
using MessangerBackend.Middlewares;
using MessangerBackend.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<MessangerContext>(options =>
    options.UseSqlServer(
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MessangerDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<SearchStatisticsService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSearchStatistics();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();