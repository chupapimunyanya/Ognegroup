using Application;
using Application.Interfaces;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Ognegroup.Infrastructure.Data;
using Ognegroup.Infrastructure.Repositories;
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

            // Adding MediatR. Specify for it to look for handlers in the Application layer assembly.
            // As an "anchor", use any class or interface from the Application layer.
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Interfaces.IProductRepository).Assembly));

            // Register global exception handler and problem details
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddControllers();
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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}