using System.Threading.Tasks;

namespace CoffeeSlotMachine.Core.Contracts
{
  public interface IUnitOfWork
  {
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    ICoinRepository Coins { get; }

    Task SaveAsync();
    Task InitializeDatabaseAsync();
    Task DeleteDatabaseAsync();
  }
}
