
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

            services.AddControllers();
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