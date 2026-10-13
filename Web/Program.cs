using Application;
using Application.Interfaces;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Ognegroup.Web.Middlewares;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DB connection
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            // Register global exception handler and problem details
            builder.Services.AddExceptionHandler<Ognegroup.Web.Middlewares.GlobalExceptionHandler>();
            builder.Services.AddProblemDetails(); // Обязательно для красивого вывода ошибок

            // Adding MediatR. Specify for it to look for handlers in the Application layer assembly.
            // As an "anchor", use any class or interface from the Application layer.
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Interfaces.IProductRepository).Assembly));

            // Register global exception handler and problem details
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ignore circular references when serializing JSON (e.g., Product -> Supplier -> Products)
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
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
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors("FrontendPolicy");

            app.UseAuthorization();

            app.UseExceptionHandler();

            app.MapControllers();

            app.Run();
        }
    }
}