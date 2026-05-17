using FluentValidation;
using GoodHamburger.Application.DependencyInjection;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Application.Interfaces.Auth;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Interfaces.Repository;
using GoodHamburger.Infra.DependencyInjection;
using GoodHamburger.Infra.Repositories;
using GoodHamburger.Infra.Services.Auth;
using GoodHamburger.Shared.Validators;
using GoodHamburger.WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GoodHamburger API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Cabeçalho Authorization usando JWT Bearer.
Digite: Bearer {seu token}",
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

    var xmlFile =
        $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});


var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

//TO-DO: ADICIONAR NO SERVICE DE D.I ACIMA
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthAppService, AuthAppService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioAppService, UsuarioAppService>();

builder.Services.AddValidatorsFromAssemblyContaining<PedidoRequestValidator>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("DevCors");

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();

//if (!app.Environment.IsDevelopment())
//{
//    app.UseHttpsRedirection();
//}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// =========================
// Migrations automáticas (opcional)
// =========================
// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var logger = services.GetRequiredService<ILogger<Program>>();
//
//     for (int i = 1; i <= 5; i++)
//     {
//         try
//         {
//             logger.LogInformation("Tentativa {Tentativa} de aplicar as migrations...", i);
//             var context = services.GetRequiredService<AppDbContext>();
//
//             context.Database.Migrate();
//
//             logger.LogInformation("Banco de dados criado e populado com sucesso!");
//             break;
//         }
//         catch
//         {
//             logger.LogWarning("Banco ainda não está pronto. Aguardando 5 segundos...");
//             Thread.Sleep(5000);
//         }
//     }
// }
app.MapGet("/", () => Results.Ok("GoodHamburger API is running!"));

app.Run();