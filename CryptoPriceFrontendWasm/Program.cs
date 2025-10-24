using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CryptoPriceFrontendWasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient para que apunte al backend
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5166/") });

await builder.Build().RunAsync();
