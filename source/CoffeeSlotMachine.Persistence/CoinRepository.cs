using CoffeeSlotMachine.Core.Contracts;
using CoffeeSlotMachine.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeSlotMachine.Persistence
{
  public class CoinRepository : ICoinRepository
  {
    private readonly ApplicationDbContext _dbContext;

    public CoinRepository(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<IEnumerable<Coin>> GetAllAsync() => await _dbContext.Coins.OrderBy(coin => coin.CoinValue).ToListAsync();

    public async Task<IEnumerable<Coin>> GetOrderedDescendingByValueAsync() => await _dbContext.Coins.OrderByDescending(coin => coin.CoinValue).ToListAsync();

    public async Task<Coin> GetByIdAsync(int id) => await _dbContext.Coins.FindAsync(id);

    public async Task AddAsync(Coin coin) => await _dbContext.Coins.AddAsync(coin);

    public async Task<bool> DeleteAsync(int id)
    {
      Coin coin = await _dbContext.Coins.FindAsync(id);
      if (coin == null)
      {
        return false;
      }
      _dbContext.Coins.Remove(coin);
      return true;
    }
  }
}
