using CoffeeSlotMachine.Core.Contracts;
using CoffeeSlotMachine.Core.DataTransferObjects;
using CoffeeSlotMachine.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeSlotMachine.Persistence
{
  public class ProductRepository : IProductRepository
  {
    private readonly ApplicationDbContext _dbContext;

    public ProductRepository(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<Product> GetByTypeNameAsync(string coffeeTypeName)
      => await _dbContext.Products
          .SingleOrDefaultAsync(product => product.Name == coffeeTypeName);

    public async Task<IEnumerable<Product>> GetWithOrders()
      => await _dbContext.Products
          .Include(product => product.Orders)
          .OrderBy(product => product.Name)
          .ToListAsync();

    public async Task<IEnumerable<Product>> GetAsync()
      => await _dbContext.Products
          .OrderBy(product => product.Name)
          .ToListAsync();

    public async Task<Product> GetByIdAsync(int id) => await _dbContext.Products.FindAsync(id);

    public void Remove(Product product) => _dbContext.Products.Remove(product);

    public async Task AddAsync(Product product) => await _dbContext.Products.AddAsync(product);

    public void Update(Product product) => _dbContext.Entry(product).State = EntityState.Modified;

    public async Task<ProductDto[]> GetProductDtosAsync()
      => await _dbContext.Products
          .Select(p => new ProductDto()
          {
            ProductId = p.Id,
            ProductName = p.Name,
            PriceInCents = p.PriceInCents,
            NrOfOrders = p.Orders.Count
          })
          .ToArrayAsync();
  }
}
