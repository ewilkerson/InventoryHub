using InventoryHub.Models;
using InventoryHub.Repositories;
using Microsoft.Extensions.Logging;
using InventoryHub.Shared.Models;

namespace InventoryHub.Services
{
    public interface IInventoryReportingService
    {
        IEnumerable<Product> GetProductsWithCategories();
        IEnumerable<object> GetTransactionHistory();
        IEnumerable<Product> GetProductsToRestock();
        IEnumerable<InventoryValueReportDto> GetInventoryValueByCategory();
    }

    public class InventoryReportingService: IInventoryReportingService
    {
        private readonly IInventoryReportingRepository _repository;
        private readonly ILogger<InventoryReportingService> _logger;

        public InventoryReportingService(IInventoryReportingRepository repository, ILogger<InventoryReportingService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public IEnumerable<Product> GetProductsWithCategories()
        {
            _logger.LogWarning("GetProductsWithCategories()");
            return  _repository.GetProductsWithCategories();
        }
        public IEnumerable<object> GetTransactionHistory() 
        {
            _logger.LogWarning("GetTransactionHistory()");
            return  _repository.GetTransactionHistory();

        }
        public IEnumerable<Product> GetProductsToRestock() 
        {
            _logger.LogWarning("GetProductsToRestock()");
            return  _repository.GetProductsToRestock();
        }
        public IEnumerable<InventoryValueReportDto> GetInventoryValueByCategory()
        {
            _logger.LogWarning("GetInventoryValueByCategory()");
            return  _repository.GetInventoryValueByCategory();
        }


    }
}
