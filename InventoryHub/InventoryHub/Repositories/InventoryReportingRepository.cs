using InventoryHub.Data;
using InventoryHub.Models;
using Microsoft.EntityFrameworkCore;
using InventoryHub.Shared.Models;

namespace InventoryHub.Repositories
{
    public interface IInventoryReportingRepository
    {
        IEnumerable<Product> GetProductsWithCategories();
        IEnumerable<object> GetTransactionHistory();
        IEnumerable<Product> GetProductsToRestock();
        IEnumerable<InventoryValueReportDto> GetInventoryValueByCategory();
    }
    public class InventoryReportingRepository : IInventoryReportingRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryReportingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. View all products with their assigned category names
        public IEnumerable<Product> GetProductsWithCategories()
        {
            // .Include() performs the INNER JOIN on the Category table[cite: 1]
            // .AsNoTracking() improves performance for read-only queries
            return _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToList();
        }

        // 2. View all transactions with the associated product details and calculate value
        public IEnumerable<object> GetTransactionHistory()
        {
            // .Select() allows you to project the data into an anonymous object (or a DTO), calculating the value on the fly
            return  _context.Transactions
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
                .ToList();
        }

        // 3. Identify products that need restocking
        public IEnumerable<Product> GetProductsToRestock()
        {
            // .Where() filters the results exactly like the SQL WHERE clause
            return  _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity <= p.ReorderLevel)
                .AsNoTracking()
                .ToList();
        }

        // 4. Calculate total inventory value per category
        public IEnumerable<InventoryValueReportDto> GetInventoryValueByCategory()
        {
            // .GroupBy() groups the records, and .Sum() performs the aggregate calculations
            return  _context.Products
                .Include(p => p.Category)
                .GroupBy(p => p.Category!.Name)
                .Select(g => new InventoryValueReportDto
                {
                    CategoryName = g.Key,
                    TotalItemsInStock = g.Sum(p => p.StockQuantity),
                    TotalInventoryValue = g.Sum(p => p.StockQuantity * p.UnitPrice)
                })
                .AsNoTracking()
                .ToList();
        }
    }

}
