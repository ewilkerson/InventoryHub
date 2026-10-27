using InventoryHub.Data;
using InventoryHub.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Repositories
{
    // Make sure the interface matches exactly
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        Product? GetById(int id);
        Product Add(Product product);
        void Update(Product product);
        void Delete(int id);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public Product? GetById(int id)
        {
            return _context.Products.Find(id);
        }

        // Note: The method is named "AddAsync" but returns a synchronous "Product". 
        // We are using synchronous SaveChanges here to match your interface signature.
        public Product Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }
    }
}
