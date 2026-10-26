using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RearqBim.Core.Interfaces;
using RearqBim.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Inyección de Servicios del Dominio y Aplicación REARQ BIM
builder.Services.AddSingleton<IMaterialService, MockMaterialService>();
builder.Services.AddSingleton<IBimStorageService, MockBimStorageService>();
builder.Services.AddSingleton<ISubscriptionService, MockSubscriptionService>();

// 2. Controladores
builder.Services.AddControllers();

// 3. Configuración de CORS para el Frontend Vue 3
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "https://app.rearqbim.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 4. Autenticación JWT Bearer
var jwtSecret = "REARQ_BIM_SUPER_SECRET_KEY_FOR_LOCAL_DEVELOPMENT_2026_JWT_TOKEN";
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// 5. Configuración de Swagger / OpenAPI con soporte para Token Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "REARQ BIM API",
        Version = "v1",
        Description = "API RESTful B2B para Marketplace Circular de Materiales de Construcción e Integración BIM (Res. 1257 / PGI-RCD)"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticación JWT. Ingrese 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "REARQ BIM API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowFrontendApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Console.WriteLine("=================================================");
Console.WriteLine("REARQ BIM API iniciada correctamente.");
Console.WriteLine("Swagger UI disponible en: http://localhost:5142/swagger");
Console.WriteLine("=================================================");

app.Run();
