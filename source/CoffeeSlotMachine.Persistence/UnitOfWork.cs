using CoffeeSlotMachine.Core.Contracts;
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CoffeeSlotMachine.Persistence
{
  public class UnitOfWork : IUnitOfWork, IDisposable
  {

    readonly ApplicationDbContext _dbContext;
    private bool _isDisposed;

    /// <summary>
    /// ConnectionString kommt von der Asp.Net Core App und ist
    /// dort in den app-Settings (JSON) gespeichert
    /// </summary>
    public UnitOfWork()
    {
      _dbContext = new ApplicationDbContext();
      Products = new ProductRepository(_dbContext);
      Orders = new OrderRepository(_dbContext);
      Coins = new CoinRepository(_dbContext);
    }

    public UnitOfWork(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
      Products = new ProductRepository(_dbContext);
      Orders = new OrderRepository(_dbContext);
      Coins = new CoinRepository(_dbContext);
    }


    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }
    public ICoinRepository Coins { get; }

    public async Task SaveAsync()
    {
      await _dbContext.SaveChangesAsync();
    }

    public async Task InitializeDatabaseAsync()
    {
      await _dbContext.Database.EnsureDeletedAsync();
      await _dbContext.Database.MigrateAsync();
    }

    public async Task DeleteDatabaseAsync()
    {
      await _dbContext.Database.EnsureDeletedAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!_isDisposed)
      {
        if (disposing)
        {
          _dbContext.Dispose();
        }
      }
      _isDisposed = true;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}
