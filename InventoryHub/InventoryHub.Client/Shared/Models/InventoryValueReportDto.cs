namespace InventoryHub.Client.Shared.Models
{
    public class InventoryValueReportDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public int TotalItemsInStock { get; set; }
        public decimal TotalInventoryValue { get; set; }
    }

    public class RestockReportDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
