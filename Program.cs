using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using WebApplication1.Data;
using WebApplication1.Exceptions;
using WebApplication1.Middleware;
using WebApplication1.Service;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["AppSettings:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
                    ValidateIssuerSigningKey = true,
                };
            });
            builder.Services.AddCors(options => 
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod());
            });

            // Adds memory cache services required for storing rate limit counters
            builder.Services.AddMemoryCache();
            // Configures IP rate limiting options from the "IpRateLimiting" section in appsettings.json
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

            // Registers the IPolicyStore with a MemoryCache implementation to store rate limit policies
            builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();

            // Registers the IRateLimitCounterStore with a MemoryCache implementation to store rate counters
            builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // Registers the rate limiting configuration which defines methods for handling rate limiting with DI
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // Registers the IProcessingStrategy as AsyncKeyLockProcessingStrategy to handle concurrent requests safely
            builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<LoggingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseIpRateLimiting();

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            exception = entry.Value.Exception?.Message,
                            duration = entry.Value.Duration.ToString()
                        })
                    };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            });

            app.MapControllers();

            app.Run();
        }
    }
}
