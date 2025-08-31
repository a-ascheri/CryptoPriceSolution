namespace CryptoPriceBackend.Services
{
    public interface IPriceService
    {
        decimal GetCurrentPrice(string baseSymbol, string targetSymbol);
    }
}