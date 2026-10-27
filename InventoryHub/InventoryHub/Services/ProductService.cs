using InventoryHub.Models;
using InventoryHub.Repositories;

namespace InventoryHub.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAllProducts();
        Product? GetProductById(int id);
        Product CreateProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int id);
    }

    public class ProductService: IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository, ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            _logger.LogInformation("Fetching all products from the database.");
            return  _repository.GetAll();
        }

        public Product? GetProductById(int id)
        {
            _logger.LogInformation("Fetching product by ID from the database.");
            return _repository.GetById(id);
        }

        public Product CreateProduct(Product product)
        {
            _logger.LogInformation("Creating a new product in the database.");
            return _repository.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            _logger.LogInformation("Updating product in the database.");
            _repository.Update(product);
        }

        public void DeleteProduct(int id)
        {
            _logger.LogInformation("Deleting product from the database.");
            _repository.Delete(id);
        }

    }
}
