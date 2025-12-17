using System.Text;
using CORE.APP.Services.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Users.APP.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString(nameof(UsersDb));
builder.Services.AddDbContext<DbContext, UsersDb>(options => options.UseSqlite(connectionString));

// Mediatr
foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
{
    builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblies(assembly));
}

builder.Services.AddSingleton<ITokenAuthService, TokenAuthService>();
builder.Configuration["SecurityKey"] = "users_microservices_security_key_2025="; // must be minimum 256 bits
// Enable JWT Bearer authentication as the default scheme.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(config =>
    {
        // Define rules for validating JWT.
        config.TokenValidationParameters = new TokenValidationParameters
        {
            // Use the builder configuration's security key to create a new symmetric security key for verifying the JWT's signature.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecurityKey"] ?? string.Empty)),

            ValidIssuer = builder.Configuration["Issuer"], // get Issuer section's value from appsettings.json
            ValidAudience = builder.Configuration["Audience"], // get Audience section's value from appsettings.json

            // These flags ensure the validation of the JWT.
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    // Define the basic information for your API.
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1"
    });

    // Add the JWT Bearer scheme to the Swagger UI so JWT can be tested in requests.
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = """
                      JWT Authorization header using the Bearer scheme.
                      Enter your JWT as: "Bearer jwt"
                      Example: "Bearer a1b2c3"
                      """
    });

    // Add the security requirement globally so all endpoints are secured unless specified otherwise.
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Configuration["SecurityKey"] = "users_microservices_security_key_2025=";

// Add services to the container.

builder.Services.AddControllers();
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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseCors();

app.Run();