using Application;
using Application.Interfaces;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Ognegroup.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Authentication;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Load environment variables from .env file
            DotNetEnv.Env.TraversePath().Load();

            var builder = WebApplication.CreateBuilder(args);

            // DB connection
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Repositories
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            // Register JWT Provider
            builder.Services.AddScoped<IJwtProvider, JwtProvider>();

            // Configure JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JWT_ISSUER"],
                        ValidAudience = builder.Configuration["JWT_AUDIENCE"],
                        // The ! operator ensures it won't be null, as we expect it to be in the .env file
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT_SECRET_KEY"]!))
                    };
                });

            // Register global exception handler and problem details (Duplicates removed)
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            // Adding MediatR. Specify for it to look for handlers in the Application layer assembly.
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Interfaces.IProductRepository).Assembly));

            // Configure Controllers and JSON Options
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Ignore circular references when serializing JSON (e.g., Product -> Supplier -> Products)
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy.AllowAnyOrigin() // For MVP allow requests from anywhere
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseExceptionHandler(); // Catches all unhandled exceptions

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors("FrontendPolicy"); // CORS must be before Auth

            // Authentication (Who you are) MUST be placed before Authorization (What you can do)
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}