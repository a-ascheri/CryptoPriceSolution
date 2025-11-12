using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CryptoPriceFrontendWasm.Models
{
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
}
