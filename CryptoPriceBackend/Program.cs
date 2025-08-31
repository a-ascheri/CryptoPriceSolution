using CryptoPriceBackend.Services;
using CryptoPriceBackend.Providers;


var builder = WebApplication.CreateBuilder(args);

// CORS configuration
// Para desarrollo: permite cualquier origen
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    // Para producción: reemplaza el dominio por el de tu frontend real
    options.AddPolicy("ProdCors", policy =>
    {
        policy.WithOrigins("https://midominio.com") // Cambia esto por tu dominio real
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped <IPriceService, PriceService>();
builder.Services.AddHttpClient<CoinGeckoProvider>();
builder.Services.AddHttpClient<ExchangeRateProvider>();
builder.Services.AddScoped<ICurrencyProvider, CoinGeckoProvider>();
builder.Services.AddScoped<ICurrencyProvider, ExchangeRateProvider>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevCors"); // Usa CORS abierto en desarrollo
}
else
{
    app.UseCors("ProdCors"); // Usa CORS restringido en producción
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
