using CoffeeSlotMachine.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeSlotMachine.Core.Contracts
{
  public interface ICoinRepository
  {
    Task<IEnumerable<Coin>> GetAllAsync();
    Task<IEnumerable<Coin>> GetOrderedDescendingByValueAsync();
    Task<Coin> GetByIdAsync(int id);
    
    Task InsertAsync(Coin coin);
    Task<bool> DeleteAsync(int id);
  }
}