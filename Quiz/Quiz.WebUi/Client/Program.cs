using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Quiz.WebUi;
using Quiz.WebUi.ApiClients;

namespace Quiz.WebUi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            var services = builder.Services;
            services.AddSingleton<IQuizApiClient, HttpQuizApiClient>(); // wsz�dzie gdzie poprosz� o wstrzykni�cie Iquiz(...) dostan� new HttpQuizApiClient

            await builder.Build().RunAsync();
        }
    }
}