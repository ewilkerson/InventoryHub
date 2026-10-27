using InventoryHub.Models;
using System.Text.Json;

namespace InventoryHub.Repositories
{
    public class ProductJSONRepository : IProductRepository
    {
        private readonly string _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "InventoryReporting.json");

        // Helper method to load and deserialize the JSON file
        private InventoryData LoadData()
        {
            if (!File.Exists(_jsonFilePath))
            {
                return new InventoryData();
            }

            var json = File.ReadAllText(_jsonFilePath);
            return JsonSerializer.Deserialize<InventoryData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new InventoryData();
        }

        public IEnumerable<Product> GetAll()
        {
            var data = LoadData();
            return data.Products ?? new List<Product>();
        }

        public  Product? GetById(int id)
        {
            var products = GetAll();
            return products.FirstOrDefault(p => p.Id == id);
        }

        public Product Add(Product product)
        {
            var products = GetAll().ToList();

            // Generate a new ID
            product.Id = products.Any() ? products.Max(p => p.Id) + 1 : 1;
            products.Add(product);

            SaveProducts(products);
            return product;
        }

        public void Update(Product product)
        {
            var products = GetAll().ToList();
            var index = products.FindIndex(p => p.Id == product.Id);

            if (index != -1)
            {
                products[index] = product;
                SaveProducts(products);
            }
        }

        public void Delete(int id)
        {
            var products = GetAll().ToList();
            var productToRemove = products.FirstOrDefault(p => p.Id == id);

            if (productToRemove != null)
            {
                products.Remove(productToRemove);
                SaveProducts(products);
            }
        }

        // Renamed to match products and added the missing semicolon
        private void SaveProducts(List<Product> products)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(products, options);

            // Ensure the directory exists before writing
            var directory = Path.GetDirectoryName(_jsonFilePath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(_jsonFilePath, json);
        }
    }
}