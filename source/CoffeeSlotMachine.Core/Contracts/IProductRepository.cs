using CoffeeSlotMachine.Core.DataTransferObjects;
using CoffeeSlotMachine.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeSlotMachine.Core.Contracts
{
  public interface IProductRepository
  {
    Task<Product> GetByTypeNameAsync(string coffeeTypeName);
    Task<IEnumerable<Product>> GetWithOrders();
    Task<IEnumerable<Product>> GetAsync();
    Task<Product> GetByIdAsync(int id);
    Task<ProductDto[]> GetProductDtosAsync();

    Task AddAsync(Product product);
    void Update(Product product);
    void Remove(Product product);
  }
}