using InventoryHub.Models;
using InventoryHub.Repositories;
using Microsoft.Extensions.Logging;

namespace InventoryHub.Services
{
    public interface IProductApiService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }

    public class ProductApiService : IProductApiService
    {
        private readonly IProductApiRepository _repository;
        private readonly ILogger<ProductApiService> _logger;

        public ProductApiService(IProductApiRepository repository, ILogger<ProductApiService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            _logger.LogInformation("Fetching all products from the database.");
            return await _repository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _logger.LogInformation("Creating new product with SKU: {SKU}", product.SKU);
            // Business Rule: Ensure stock is not negative on creation
            if (product.StockQuantity < 0) throw new ArgumentException("Stock cannot be negative.");

            return await _repository.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            _logger.LogInformation("Updating product ID: {Id}", product.Id);
            await _repository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            _logger.LogWarning("Deleting product ID: {Id}", id);
            await _repository.DeleteAsync(id);
        }
    }
}
