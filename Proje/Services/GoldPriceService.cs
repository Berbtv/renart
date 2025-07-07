using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Proje.Services
{
    public class GoldPriceService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoldPriceService> _logger;
        private double? _cachedGoldPrice;
        private DateTime _lastCacheTime = DateTime.MinValue;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5); // 5 dakika cache
        private const string API_KEY = "goldapi-1jlsbksmcswccuk-io";

        public GoldPriceService(HttpClient httpClient, ILogger<GoldPriceService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<double> GetGoldPricePerGramAsync()
        {
            // Cache kontrolü
            if (_cachedGoldPrice.HasValue && DateTime.UtcNow - _lastCacheTime < _cacheExpiration)
            {
                return _cachedGoldPrice.Value;
            }

            try
            {
                // GoldAPI'den altın fiyatını al
                var request = new HttpRequestMessage(HttpMethod.Get, "https://www.goldapi.io/api/XAU/USD");
                request.Headers.Add("x-access-token", API_KEY);
                

                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API Response: {Content}", content);
                    
                    try
                    {
                        // Önce dynamic olarak parse edelim
                        var jsonDoc = JsonDocument.Parse(content);
                        var root = jsonDoc.RootElement;
                        
                        // price_gram_24k değerini al
                        if (root.TryGetProperty("price_gram_24k", out var priceGram24kElement) && 
                            priceGram24kElement.TryGetDouble(out var priceGram24k))
                        {
                            _cachedGoldPrice = priceGram24k;
                            _lastCacheTime = DateTime.UtcNow;
                            
                            _logger.LogInformation("Altın fiyatı başarıyla alındı: ${PricePerGram:F2}/gram (24k)", priceGram24k);
                            return priceGram24k;
                        }
                        else if (root.TryGetProperty("price", out var priceElement) && 
                                 priceElement.TryGetDouble(out var price))
                        {
                            // Troy ounce'dan gram'a çevir
                            var pricePerGram = price / 31.1035;
                            _cachedGoldPrice = pricePerGram;
                            _lastCacheTime = DateTime.UtcNow;
                            
                            _logger.LogInformation("Altın fiyatı troy ounce'dan hesaplandı: ${PricePerGram:F2}/gram", pricePerGram);
                            return pricePerGram;
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "JSON parse hatası");
                    }
                }

                // API başarısız olursa varsayılan değer kullan
                _logger.LogWarning("Altın fiyatı API'den alınamadı, varsayılan değer kullanılıyor");
                _cachedGoldPrice = 65.0; // Varsayılan gram başına USD
                _lastCacheTime = DateTime.UtcNow;
                return _cachedGoldPrice.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Altın fiyatı alınırken hata oluştu");
                
                // Hata durumunda varsayılan değer
                if (!_cachedGoldPrice.HasValue)
                {
                    _cachedGoldPrice = 65.0; // Varsayılan gram başına USD
                    _lastCacheTime = DateTime.UtcNow;
                }
                
                return _cachedGoldPrice.Value;
            }
        }

        public double CalculateProductPrice(double popularityScore, double weight, double goldPricePerGram)
        {
            return (popularityScore + 1) * weight * goldPricePerGram;
        }
    }

    // GoldAPI Response modeli - sadece gerekli property'ler
    public class GoldApiResponse
    {
        [JsonPropertyName("price")]
        public double Price { get; set; }
        
        [JsonPropertyName("price_gram_24k")]
        public double PriceGram24k { get; set; }
    }
} 