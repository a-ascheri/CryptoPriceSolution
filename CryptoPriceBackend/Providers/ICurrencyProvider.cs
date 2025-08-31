namespace CryptoPriceBackend.Providers
{
    public interface ICurrencyProvider
    {
        decimal GetPrice(string baseSymbol, string targetSymbol);
    }
}