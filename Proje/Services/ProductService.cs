using System.Text.Json;
using Proje.Models;

namespace Proje.Services
{
    public class ProductService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProductService> _logger;
        private readonly GoldPriceService _goldPriceService;
        private List<Product>? _products;
        private DateTime _lastCacheTime = DateTime.MinValue;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public ProductService(IWebHostEnvironment environment, ILogger<ProductService> logger, GoldPriceService goldPriceService)
        {
            _environment = environment;
            _logger = logger;
            _goldPriceService = goldPriceService;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            // Cache kontrolü - 5 dakikada bir yenile
            if (_products != null && DateTime.UtcNow - _lastCacheTime < _cacheExpiration)
                return _products;

            try
            {
                var jsonPath = Path.Combine(_environment.WebRootPath, "data", "products.json");
                
                if (!File.Exists(jsonPath))
                {
                    _logger.LogWarning("products.json dosyası bulunamadı: {Path}", jsonPath);
                    return new List<Product>();
                }

                var jsonContent = await File.ReadAllTextAsync(jsonPath);
                var products = JsonSerializer.Deserialize<List<Product>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (products == null)
                    return new List<Product>();

                // Altın fiyatını al
                var goldPricePerGram = await _goldPriceService.GetGoldPricePerGramAsync();

                // Her ürün için fiyat hesapla
                foreach (var product in products)
                {
                    product.Price = _goldPriceService.CalculateProductPrice(
                        product.PopularityScore, 
                        product.Weight, 
                        goldPricePerGram
                    );
                    
                    product.PriceFormatted = $"${product.Price:F2}";
                }

                _products = products;
                _lastCacheTime = DateTime.UtcNow;
                
                return _products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürünler yüklenirken hata oluştu");
                return _products ?? new List<Product>();
            }
        }

        public async Task<Product?> GetProductByNameAsync(string name)
        {
            var products = await GetProductsAsync();
            return products.FirstOrDefault(p => p.Name == name);
        }
    }
} 