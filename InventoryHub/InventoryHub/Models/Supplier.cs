using System.ComponentModel.DataAnnotations;

namespace InventoryHub.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        
        [Required, StringLength(100)]
        public string CompanyName { get; set; } = string.Empty;
        
        [Required, EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;
    }
}
