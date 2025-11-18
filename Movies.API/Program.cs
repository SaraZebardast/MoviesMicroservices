using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add services to the IoC container.
var connectionString = builder.Configuration.GetConnectionString(nameof(MoviesDb));
builder.Services.AddDbContext<DbContext, MoviesDb>(options => options.UseSqlite(connectionString));

// MediatR
foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
{
    builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblies(assembly));
}

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();