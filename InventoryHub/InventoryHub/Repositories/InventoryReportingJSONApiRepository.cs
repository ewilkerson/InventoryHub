using InventoryHub.Models;
using InventoryHub.Repositories;
using System.Text.Json;

namespace InventoryHub.Repository
{
    public class InventoryReportingJSONApiRepository : IInventoryReportingApiRepository
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

        public async Task<IEnumerable<Product>> GetProductsWithCategoriesAsync()
        {
            var data = await LoadDataAsync();

            // Perform an in-memory join to populate the Category navigation property
            foreach (var product in data.Products)
            {
                product.Category = data.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }

            return data.Products;
        }

        public async Task<IEnumerable<object>> GetTransactionHistoryAsync()
        {
            var data = await LoadDataAsync();

            // Project the data into an anonymous object matching the repository's output format
            return data.Transactions
                .OrderByDescending(t => t.TransactionDate)
                .Select(t =>
                {
                    var product = data.Products.FirstOrDefault(p => p.Id == t.ProductId);
                    return new
                    {
                        t.Id,
                        t.TransactionDate,
                        t.TransactionType,
                        t.Quantity,
                        ProductName = product?.Name ?? "Unknown",
                        SKU = product?.SKU ?? "N/A",
                        TotalTransactionValue = (product?.UnitPrice ?? 0) * t.Quantity
                    };
                });
        }

        public async Task<IEnumerable<Product>> GetProductsToRestockAsync()
        {
            var data = await LoadDataAsync();

            // Filter products where stock is at or below the reorder level
            var productsToRestock = data.Products
                .Where(p => p.StockQuantity <= p.ReorderLevel)
                .ToList();

            // Populate the Category navigation property for the filtered products
            foreach (var product in productsToRestock)
            {
                product.Category = data.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }

            return productsToRestock;
        }

        public async Task<IEnumerable<object>> GetInventoryValueByCategoryAsync()
        {
            var data = await LoadDataAsync();

            // Populate Category navigation property to group by category name
            foreach (var product in data.Products)
            {
                product.Category = data.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }

            // Group by Category Name and calculate totals
            return data.Products
                .Where(p => p.Category != null)
                .GroupBy(p => p.Category!.Name)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    TotalItemsInStock = g.Sum(p => p.StockQuantity),
                    TotalInventoryValue = g.Sum(p => p.StockQuantity * p.UnitPrice)
                });
        }

    }
}
