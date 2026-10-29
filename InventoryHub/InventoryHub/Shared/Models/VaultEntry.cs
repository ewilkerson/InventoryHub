namespace InventoryHub.Shared.Models
{
    public class VaultEntry
    {
        public string ServiceName { get; set; }
        public string Username { get; set; }
        public string EncryptedPassword { get; set; } // Encrypted via VaultCryptoService
    }
}
