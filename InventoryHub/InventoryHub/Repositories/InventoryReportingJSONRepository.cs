using InventoryHub.Models;
using InventoryHub.Repositories;
using System.Text.Json;
using InventoryHub.Shared.Models;

namespace InventoryHub.Repository
{
    public class InventoryReportingJSONRepository : IInventoryReportingRepository
    {
       // private readonly string _jsonFilePath = "InventoryReporting.json";
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

        public  IEnumerable<Product> GetProductsWithCategories()
        {
            var data = LoadData();

            // Perform an in-memory join to populate the Category navigation property
            
            foreach (var product in data.Products)
            {
                product.Category = data.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }

            return data.Products;
        }

        public  IEnumerable<object> GetTransactionHistory()
        {
            var data = LoadData();

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

        public IEnumerable<Product> GetProductsToRestock()
        {
            var data = LoadData();

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

        public IEnumerable<InventoryValueReportDto> GetInventoryValueByCategory()
        {
            var data = LoadData();

            // Populate Category navigation property to group by category name
            foreach (var product in data.Products)
            {
                product.Category = data.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }

            // Group by Category Name and calculate totals
            return data.Products
                .Where(p => p.Category != null)
                .GroupBy(p => p.Category!.Name)
                .Select(g => new InventoryValueReportDto
                {
                    CategoryName = g.Key,
                    TotalItemsInStock = g.Sum(p => p.StockQuantity),
                    TotalInventoryValue = g.Sum(p => p.StockQuantity * p.UnitPrice)
                });
        }

    }
}
