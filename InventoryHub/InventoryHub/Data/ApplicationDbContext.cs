using InventoryHub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryHub.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // DbSets for all Core Layers
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. One-to-Many Relationship: Category -> Products
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevents deleting a category if products exist

            // 2. One-to-Many Relationship: Product -> Transactions
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Product)
                .WithMany() // Assuming we don't need a Transactions list on the Product model right now
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting a product deletes its transaction history

            // 3. Precision handling for financial data
            modelBuilder.Entity<Product>()
                .Property(p => p.UnitPrice)
                .HasColumnType("decimal(18,2)");

            // 4. Ensure SKU is unique at the database level
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();
        }

    }
}
