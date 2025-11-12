using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CryptoPriceFrontendWasm;
using CryptoPriceFrontendWasm.Services;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient para que apunte al backend
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5166/") });

// Registrar servicios de Radzen
builder.Services.AddRadzenComponents();

// Registrar servicios personalizados
builder.Services.AddScoped<BondDataService>();

await builder.Build().RunAsync();
