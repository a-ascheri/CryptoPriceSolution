using Microsoft.AspNetCore.Mvc;
using CryptoPriceBackend.Services;

namespace CryptoPriceBackend.Controllers
{
    [ApiController]
[Route("api/[controller]")]
public class PricesController : ControllerBase
{
    private readonly IPriceService _priceService;

    public PricesController(IPriceService priceService)
    {
        _priceService = priceService;
    }

    [HttpGet("{baseSymbol}/{targetSymbol}")]
    public IActionResult Get(string baseSymbol, string targetSymbol)
    {
        var price = _priceService.GetCurrentPrice(baseSymbol, targetSymbol);
        return Ok(new { baseSymbol, targetSymbol, price });
    }
}
}