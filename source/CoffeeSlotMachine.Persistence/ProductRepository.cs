using CoffeeSlotMachine.Core.Contracts;
using CoffeeSlotMachine.Core.DataTransferObjects;
using CoffeeSlotMachine.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeSlotMachine.Persistence
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Product GetByTypeName(string coffeeTypeName)
        {
            return _dbContext.Products.SingleOrDefault(product => product.Name == coffeeTypeName);
        }

        public IEnumerable<Product> GetWithOrders()
        {
            return _dbContext.Products.Include(product => product.Orders)
                .OrderBy(product => product.Name).ToList();

        }

        public IEnumerable<Product> Get()
        {
            return _dbContext.Products.OrderBy(product => product.Name).ToList();
        }

        public Product GetById(int id)
        {
            return _dbContext.Products.Find(id);
        }

        public void Remove(Product product)
        {
            _dbContext.Products.Remove(product);
        }

        public void Add(Product product)
        {
            _dbContext.Products.Add(product);
        }

        public void Update(Product product)
        {
            _dbContext.Entry(product).State = EntityState.Modified;
        }

        public ProductDto[] GetProductDtos()
        {
            return _dbContext
                .Products
                .Select(p => new ProductDto()
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    PriceInCents = p.PriceInCents,
                    NrOfOrders = p.Orders.Count
                }).ToArray();
        }
    }
}
