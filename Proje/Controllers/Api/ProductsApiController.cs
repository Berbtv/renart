using Microsoft.AspNetCore.Mvc;
using Proje.Models;
using Proje.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Proje.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly GoldPriceService _goldPriceService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ProductService productService, GoldPriceService goldPriceService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _goldPriceService = goldPriceService;
            _logger = logger;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Product>>>> GetProducts()
        {
            try
            {
                var products = await _productService.GetProductsAsync();
                return Ok(new ApiResponse<List<Product>>
                {
                    Success = true,
                    Data = products,
                    Message = "Ürünler başarıyla getirildi"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürünler getirilirken hata oluştu");
                return StatusCode(500, new ApiResponse<List<Product>>
                {
                    Success = false,
                    Message = "Ürünler getirilirken bir hata oluştu"
                });
            }
        }

        // GET: api/products/{name}
        [HttpGet("{name}")]
        public async Task<ActionResult<ApiResponse<Product>>> GetProduct(string name)
        {
            try
            {
                var product = await _productService.GetProductByNameAsync(name);
                
                if (product == null)
                {
                    return NotFound(new ApiResponse<Product>
                    {
                        Success = false,
                        Message = $"'{name}' adında ürün bulunamadı"
                    });
                }

                return Ok(new ApiResponse<Product>
                {
                    Success = true,
                    Data = product,
                    Message = "Ürün başarıyla getirildi"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün getirilirken hata oluştu: {Name}", name);
                return StatusCode(500, new ApiResponse<Product>
                {
                    Success = false,
                    Message = "Ürün getirilirken bir hata oluştu"
                });
            }
        }

        // GET: api/products/search?query={query}
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<List<Product>>>> SearchProducts([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new ApiResponse<List<Product>>
                    {
                        Success = false,
                        Message = "Arama sorgusu boş olamaz"
                    });
                }

                var products = await _productService.GetProductsAsync();
                var filteredProducts = products.Where(p => 
                    p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

                return Ok(new ApiResponse<List<Product>>
                {
                    Success = true,
                    Data = filteredProducts,
                    Message = $"'{query}' için {filteredProducts.Count} ürün bulundu"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün arama sırasında hata oluştu: {Query}", query);
                return StatusCode(500, new ApiResponse<List<Product>>
                {
                    Success = false,
                    Message = "Ürün arama sırasında bir hata oluştu"
                });
            }
        }

        private double RoundToHalf(double value)
{
    return Math.Round(value * 2, MidpointRounding.AwayFromZero) / 2.0;
}

        // GET: api/products/gold-price
        [HttpGet("gold-price")]
        public async Task<ActionResult<ApiResponse<object>>> GetGoldPrice()
        {
            try
            {
                var goldPricePerGram = await _goldPriceService.GetGoldPricePerGramAsync();
                
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = new
                    {
                        pricePerGram = goldPricePerGram,
                        pricePerGramFormatted = $"${goldPricePerGram:F2}",
                        pricePerOunce = goldPricePerGram * 31.1035,
                        pricePerOunceFormatted = $"${goldPricePerGram * 31.1035:F2}",
                        lastUpdated = DateTime.UtcNow,
                        note = "24k altın gram başına fiyat"
                    },
                    Message = "Altın fiyatı başarıyla getirildi"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Altın fiyatı getirilirken hata oluştu");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Altın fiyatı getirilirken bir hata oluştu"
                });
            }
        }

        // GET: api/products/filter?minPrice=100&maxPrice=500&minPopularity=0.2&maxPopularity=0.8
        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<List<Product>>>> FilterProducts(
            [FromQuery] double? minPrice,
            [FromQuery] double? maxPrice,
            [FromQuery] double? minPopularity,
            [FromQuery] double? maxPopularity)
        {
            try
            {
                var products = await _productService.GetProductsAsync();
                
                _logger.LogInformation("Filter request - minPrice: {MinPrice}, maxPrice: {MaxPrice}, minPopularity: {MinPopularity}, maxPopularity: {MaxPopularity}", 
                    minPrice, maxPrice, minPopularity, maxPopularity);
                _logger.LogInformation("Total products before filtering: {Count}", products.Count);

                if (minPrice.HasValue)
                {
                    products = products.Where(p => p.Price >= minPrice.Value).ToList();
                    _logger.LogInformation("After minPrice filter: {Count} products", products.Count);
                }
                if (maxPrice.HasValue)
                {
                    products = products.Where(p => p.Price <= maxPrice.Value).ToList();
                    _logger.LogInformation("After maxPrice filter: {Count} products", products.Count);
                }
                
                if (minPopularity.HasValue)
                {
                    products = products.Where(p => Math.Round(p.PopularityScore * 5) >= Math.Round(minPopularity.Value * 5)).ToList();
                }
                if (maxPopularity.HasValue)
                {
                    products = products.Where(p => Math.Round(p.PopularityScore * 5) <= Math.Round(maxPopularity.Value * 5)).ToList();
                }

                return Ok(new ApiResponse<List<Product>>
                {
                    Success = true,
                    Data = products,
                    Message = $"Filtreye uyan {products.Count} ürün bulundu."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Filtreli ürünler getirilirken hata oluştu");
                return StatusCode(500, new ApiResponse<List<Product>>
                {
                    Success = false,
                    Message = "Filtreli ürünler getirilirken bir hata oluştu"
                });
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> ProductList(string[]? colors, double? minRating)
        {
            // ProductService ile ürünleri al
            var products = await _productService.GetProductsAsync();

            // Renk filtresi uygula
            if (colors != null && colors.Length > 0)
            {
                products = products.Where(p =>
                    (colors.Contains("Yellow Gold") && !string.IsNullOrEmpty(p.Images.Yellow)) ||
                    (colors.Contains("White Gold") && !string.IsNullOrEmpty(p.Images.White)) ||
                    (colors.Contains("Rose Gold") && !string.IsNullOrEmpty(p.Images.Rose))
                ).ToList();
            }
            // Puan filtresi uygula (0-1 arası PopularityScore'u 5'lik skala ile karşılaştır)
            if (minRating.HasValue)
            {
                double minScore = minRating.Value / 5.0;
                products = products.Where(p => p.PopularityScore >= minScore).ToList();
            }

            return Ok(products);
        }
    }
} 