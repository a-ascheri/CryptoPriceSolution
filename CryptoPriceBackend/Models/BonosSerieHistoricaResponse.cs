using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CryptoPriceBackend.Models
{
    /// <summary>
    /// Representa un punto de datos histórico de un bono
    /// </summary>
    public class BonoHistoricoDataPoint
    {
        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; }

        [JsonPropertyName("precio")]
        public double Precio { get; set; }

        [JsonPropertyName("variacion")]
        public double? Variacion { get; set; }

        [JsonPropertyName("apertura")]
        public double? Apertura { get; set; }

        [JsonPropertyName("maximo")]
        public double? Maximo { get; set; }

        [JsonPropertyName("minimo")]
        public double? Minimo { get; set; }

        [JsonPropertyName("volumen")]
        public double? Volumen { get; set; }
    }

    /// <summary>
    /// Respuesta de la serie histórica de un bono
    /// </summary>
    public class BonosSerieHistoricaResponse
    {
        [JsonPropertyName("simbolo")]
        public string Simbolo { get; set; } = string.Empty;

        [JsonPropertyName("mercado")]
        public string Mercado { get; set; } = string.Empty;

        [JsonPropertyName("fechaDesde")]
        public DateTime FechaDesde { get; set; }

        [JsonPropertyName("fechaHasta")]
        public DateTime FechaHasta { get; set; }

        [JsonPropertyName("datos")]
        public List<BonoHistoricoDataPoint> Datos { get; set; } = new();

        [JsonPropertyName("moneda")]
        public string? Moneda { get; set; }
    }

    /// <summary>
    /// Respuesta de la API de InvertirOnline para series históricas
    /// </summary>
    public class InvertirOnlineSerieHistoricaItem
    {
        [JsonPropertyName("ultimoPrecio")]
        public double UltimoPrecio { get; set; }

        [JsonPropertyName("variacion")]
        public double? Variacion { get; set; }

        [JsonPropertyName("apertura")]
        public double? Apertura { get; set; }

        [JsonPropertyName("maximo")]
        public double? Maximo { get; set; }

        [JsonPropertyName("minimo")]
        public double? Minimo { get; set; }

        [JsonPropertyName("fechahora")]
        public string? FechaHora { get; set; }

        [JsonPropertyName("tendencia")]
        public string? Tendencia { get; set; }

        [JsonPropertyName("cierreAnterior")]
        public double? CierreAnterior { get; set; }

        [JsonPropertyName("montoOperado")]
        public double? MontoOperado { get; set; }

        [JsonPropertyName("volumenNominal")]
        public double? VolumenNominal { get; set; }

        [JsonPropertyName("moneda")]
        public string? Moneda { get; set; }
    }
}
