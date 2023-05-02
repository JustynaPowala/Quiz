
using Microsoft.AspNetCore.Cors.Infrastructure;
using Quiz.WebApi.Controllers;

namespace Quiz.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            // Add services to the container.

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowedCorsOrigins",

                    corsPolicyBuilder => 
                    {
                        corsPolicyBuilder
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    }
                    );

            });

            
            services.AddSingleton<ICategoriesProvider, HardcodedCategoriesProvider>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.UseCors("AllowedCorsOrigins");

            app.MapControllers();

            app.Run();
        }
    }
}