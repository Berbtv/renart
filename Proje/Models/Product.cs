using System.Text.Json.Serialization;
using System;

namespace Proje.Models
{
    public class Product
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("popularityScore")]
        public double PopularityScore { get; set; }
        
        [JsonPropertyName("popularityScoreOutOf5")]
        public string PopularityScoreOutOf5 
        { 
            get 
            {
                var score = Math.Round(PopularityScore * 5);
                return $"{score}/5";
            }
        }
        
        [JsonPropertyName("weight")]
        public double Weight { get; set; }
        
        [JsonPropertyName("images")]
        public ProductImages Images { get; set; } = new ProductImages();
        
        [JsonPropertyName("price")]
        public double Price { get; set; }
        
        [JsonPropertyName("priceFormatted")]
        public string PriceFormatted { get; set; } = string.Empty;
    }

    public class ProductImages
    {
        [JsonPropertyName("yellow")]
        public string Yellow { get; set; } = string.Empty;
        
        [JsonPropertyName("rose")]
        public string Rose { get; set; } = string.Empty;
        
        [JsonPropertyName("white")]
        public string White { get; set; } = string.Empty;
    }
} 