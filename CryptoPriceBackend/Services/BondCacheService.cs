using System;
using System.Collections.Concurrent;
using CryptoPriceBackend.Models;

namespace CryptoPriceBackend.Services
{
    /// <summary>
    /// Servicio de caché en memoria para datos de bonos
    /// </summary>
    public class BondCacheService
    {
        private readonly ConcurrentDictionary<string, CachedItem<BonosSerieHistoricaResponse>> _cache = new();
        private readonly TimeSpan _cacheExpiration;

        public BondCacheService(TimeSpan? cacheExpiration = null)
        {
            _cacheExpiration = cacheExpiration ?? TimeSpan.FromMinutes(5);
        }

        public BonosSerieHistoricaResponse? Get(string key)
        {
            if (_cache.TryGetValue(key, out var cachedItem))
            {
                if (DateTime.UtcNow < cachedItem.Expiration)
                {
                    Console.WriteLine($"[BondCache] Cache HIT para: {key}");
                    return cachedItem.Value;
                }
                else
                {
                    // Eliminar entrada expirada
                    _cache.TryRemove(key, out _);
                    Console.WriteLine($"[BondCache] Cache EXPIRED para: {key}");
                }
            }
            
            Console.WriteLine($"[BondCache] Cache MISS para: {key}");
            return null;
        }

        public void Set(string key, BonosSerieHistoricaResponse value)
        {
            var cachedItem = new CachedItem<BonosSerieHistoricaResponse>
            {
                Value = value,
                Expiration = DateTime.UtcNow.Add(_cacheExpiration)
            };
            
            _cache[key] = cachedItem;
            Console.WriteLine($"[BondCache] Cached: {key} (expira en {_cacheExpiration.TotalMinutes} minutos)");
        }

        public void Clear()
        {
            _cache.Clear();
            Console.WriteLine("[BondCache] Cache limpiado");
        }

        public static string GenerateKey(string mercado, string simbolo, DateTime fechaDesde, DateTime fechaHasta)
        {
            return $"{mercado}_{simbolo}_{fechaDesde:yyyyMMdd}_{fechaHasta:yyyyMMdd}";
        }

        private class CachedItem<T>
        {
            public T Value { get; set; } = default!;
            public DateTime Expiration { get; set; }
        }
    }
}
