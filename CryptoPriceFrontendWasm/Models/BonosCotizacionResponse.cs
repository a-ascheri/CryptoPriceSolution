using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CryptoPriceFrontendWasm.Models
{
    public class BonosCotizacionResponse
    {
        [JsonPropertyName("ultimoPrecio")]
        public double UltimoPrecio { get; set; }
        [JsonPropertyName("variacion")]
        public double Variacion { get; set; }
        [JsonPropertyName("apertura")]
        public double Apertura { get; set; }
        [JsonPropertyName("maximo")]
        public double Maximo { get; set; }
        [JsonPropertyName("minimo")]
        public double Minimo { get; set; }
        [JsonPropertyName("fechaHora")]
        public DateTime FechaHora { get; set; }
        [JsonPropertyName("tendencia")]
        public string? Tendencia { get; set; }
        [JsonPropertyName("cierreAnterior")]
        public double CierreAnterior { get; set; }
        [JsonPropertyName("montoOperado")]
        public double MontoOperado { get; set; }
        [JsonPropertyName("volumenNominal")]
        public double VolumenNominal { get; set; }
        [JsonPropertyName("precioPromedio")]
        public double PrecioPromedio { get; set; }
        [JsonPropertyName("moneda")]
        public string? Moneda { get; set; }
        [JsonPropertyName("precioAjuste")]
        public double PrecioAjuste { get; set; }
        [JsonPropertyName("interesesAbiertos")]
        public double InteresesAbiertos { get; set; }
        [JsonPropertyName("puntas")]
        public List<Punta>? Puntas { get; set; }
        [JsonPropertyName("cantidadOperaciones")]
        public int CantidadOperaciones { get; set; }
        [JsonPropertyName("descripcionTitulo")]
        public string? DescripcionTitulo { get; set; }
        [JsonPropertyName("plazo")]
        public string? Plazo { get; set; }
        [JsonPropertyName("laminaMinima")]
        public double LaminaMinima { get; set; }
        [JsonPropertyName("lote")]
        public double Lote { get; set; }
    }

    public class Punta
    {
        [JsonPropertyName("cantidadCompra")]
        public double CantidadCompra { get; set; }
        [JsonPropertyName("precioCompra")]
        public double PrecioCompra { get; set; }
        [JsonPropertyName("precioVenta")]
        public double PrecioVenta { get; set; }
        [JsonPropertyName("cantidadVenta")]
        public double CantidadVenta { get; set; }
    }
}
