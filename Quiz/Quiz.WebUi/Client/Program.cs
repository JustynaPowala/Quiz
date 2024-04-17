using Blazored.Toast;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Quiz.WebUi;
using Quiz.WebUi.ApiClients;
using Syncfusion.Blazor;

namespace Quiz.WebUi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            builder.Services.AddBlazoredToast();

            var services = builder.Services;
            services.AddSingleton<IQuizApiClient, HttpQuizApiClient>(); 
            services.AddTransient<ErrorHandlingHttpMessageHandler>();
            services
                .AddHttpClient("QuizHttpClient",(sp,client) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    client.BaseAddress = new Uri(configuration["HttpClients:Quiz"]);
                })
                .AddHttpMessageHandler<ErrorHandlingHttpMessageHandler>();
            await builder.Build().RunAsync();
        }
    }
}