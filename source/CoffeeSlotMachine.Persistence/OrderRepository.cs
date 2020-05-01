using CoffeeSlotMachine.Core.Contracts;
using CoffeeSlotMachine.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CoffeeSlotMachine.Persistence
{
  public class OrderRepository : IOrderRepository
  {
    private readonly ApplicationDbContext _dbContext;

    public OrderRepository(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<IEnumerable<Order>> GetAllAsync() => await _dbContext.Orders.ToListAsync();

    public async Task<Order> GetByIdAsync(int id) => await _dbContext.Orders.FirstOrDefaultAsync(order => order.Id == id);

    public async Task InsertAsync(Order order) => await _dbContext.Orders.AddAsync(order);

    public async Task<IEnumerable<Order>> GetAllWithProductAsync()
     => await _dbContext.Orders
            .Include(order => order.Product)
            .OrderByDescending(order => order.Time)
            .ToListAsync();

    public async Task<Order> GetByIdWithProductAndCoinsAsync(int id)
    => await _dbContext.Orders
          .Include(o => o.Product)
          .SingleOrDefaultAsync(o => o.Id == id);

    public async Task<bool> DeleteAsync(int id)
    {
      Order order = await _dbContext.Orders.FindAsync(id);
      if (order == null)
      {
        return false;
      }
      _dbContext.Orders.Remove(order);
      return true;
    }
  }
}
