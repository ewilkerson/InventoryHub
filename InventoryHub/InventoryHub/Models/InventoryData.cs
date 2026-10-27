namespace InventoryHub.Models
{
    public class InventoryData
    {
        public List<Category> Categories { get; set; } = new();
        public List<Product> Products { get; set; } = new();
        public List<Supplier> Suppliers { get; set; } = new();
        public List<Transaction> Transactions { get; set; } = new();
    }
}
