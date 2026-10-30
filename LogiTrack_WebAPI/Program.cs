using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Practica_net1_LogiTrack.Repositorio;
using LogiTrack_WebAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Base_Inventario")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=LogiTrackDB;Trusted_Connection=True;";

// Capa de datos (EF Core)
builder.Services.AddDbContext<LogiTrackContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<EfCoreRepositorio>();
builder.Services.AddScoped<DapperRepositorio>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthService>();

// Autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", p => p.RequireRole("Administrador"));
    options.AddPolicy("Bodeguero", p => p.RequireRole("Bodeguero"));
    options.AddPolicy("Cliente", p => p.RequireRole("Cliente"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
