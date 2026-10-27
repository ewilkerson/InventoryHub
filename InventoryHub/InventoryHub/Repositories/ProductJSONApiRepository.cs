using InventoryHub.Models;
using System.Text.Json;

namespace InventoryHub.Repositories
{
    public class ProductJSONApiRepository : IProductApiRepository
    {
        private readonly string _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "InventoryReporting.json");


        // Helper method to load and deserialize the JSON file
        private async Task<InventoryData> LoadDataAsync()
        {
            if (!File.Exists(_jsonFilePath))
            {
                return new InventoryData();
            }

            var json = await File.ReadAllTextAsync(_jsonFilePath);
            return JsonSerializer.Deserialize<InventoryData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new InventoryData();
        }


        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var data = await LoadDataAsync();
            return data.Products ?? new List<Product>();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var products = await GetAllAsync();
            return products.FirstOrDefault(p => p.Id == id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            var products = (await GetAllAsync()).ToList();

            // Generate a new ID
            product.Id = products.Any() ? products.Max(p => p.Id) + 1 : 1;
            products.Add(product);

            await SaveProductsAsync(products);
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            var products = (await GetAllAsync()).ToList();
            var index = products.FindIndex(p => p.Id == product.Id);

            if (index != -1)
            {
                products[index] = product;
                await SaveProductsAsync(products);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var products = (await GetAllAsync()).ToList();
            var productToRemove = products.FirstOrDefault(p => p.Id == id);

            if (productToRemove != null)
            {
                products.Remove(productToRemove);
                await SaveProductsAsync(products);
            }
        }

        // Renamed to match products and added the missing semicolon
        private async Task SaveProductsAsync(List<Product> products)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(products, options);

            // Ensure the directory exists before writing
            var directory = Path.GetDirectoryName(_jsonFilePath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(_jsonFilePath, json);
        }
    }
}