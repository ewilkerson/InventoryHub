using System.ComponentModel.DataAnnotations;

namespace InventoryHub.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [Required]
        public string TransactionType { get; set; } = "Sale"; // or "Restock"
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}
