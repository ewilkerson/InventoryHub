using Microsoft.EntityFrameworkCore;
using InventoryHub.Models;
using InventoryHub.Data;

namespace InventoryHub.Repositories
{
    public interface IInventoryReportingApiRepository
    {
        Task<IEnumerable<Product>> GetProductsWithCategoriesAsync();
        Task<IEnumerable<object>> GetTransactionHistoryAsync();
        Task<IEnumerable<Product>> GetProductsToRestockAsync();
        Task<IEnumerable<object>> GetInventoryValueByCategoryAsync();
    }
    public class InventoryReportingApiRepository : IInventoryReportingApiRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryReportingApiRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. View all products with their assigned category names
        public async Task<IEnumerable<Product>> GetProductsWithCategoriesAsync()
        {
            // .Include() performs the INNER JOIN on the Category table[cite: 1]
            // .AsNoTracking() improves performance for read-only queries
            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        // 2. View all transactions with the associated product details and calculate value
        public async Task<IEnumerable<object>> GetTransactionHistoryAsync()
        {
            // .Select() allows you to project the data into an anonymous object (or a DTO), calculating the value on the fly
            return await _context.Transactions
                .Include(t => t.Product)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new
                {
                    t.Id,
                    t.TransactionDate,
                    t.TransactionType,
                    t.Quantity,
                    ProductName = t.Product!.Name,
                    t.Product.SKU,
                    TotalTransactionValue = t.Product.UnitPrice * t.Quantity
                })
                .AsNoTracking()
                .ToListAsync();
        }

        // 3. Identify products that need restocking
        public async Task<IEnumerable<Product>> GetProductsToRestockAsync()
        {
            // .Where() filters the results exactly like the SQL WHERE clause
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity <= p.ReorderLevel)
                .AsNoTracking()
                .ToListAsync();
        }

        // 4. Calculate total inventory value per category
        public async Task<IEnumerable<object>> GetInventoryValueByCategoryAsync()
        {
            // .GroupBy() groups the records, and .Sum() performs the aggregate calculations
            return await _context.Products
                .Include(p => p.Category)
                .GroupBy(p => p.Category!.Name)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    TotalItemsInStock = g.Sum(p => p.StockQuantity),
                    TotalInventoryValue = g.Sum(p => p.StockQuantity * p.UnitPrice)
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
