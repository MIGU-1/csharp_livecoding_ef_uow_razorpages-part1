using CoffeeSlotMachine.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeSlotMachine.Core.Contracts
{
    public interface IOrderRepository
    {
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order> GetByIdAsync(int id);
    Task InsertAsync(Order order);
    Task<IEnumerable<Order>> GetAllWithProductAsync();
    Task<Order> GetByIdWithProductAndCoinsAsync(int id);
    
    Task<bool> DeleteAsync(int id);
  }
}