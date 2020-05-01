using CoffeeSlotMachine.Core;
using CoffeeSlotMachine.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeSlotMachine.ControllerTest
{
  [TestClass]
  public class ControllerTest
  {
    [TestInitialize]
    public void MyTestInitialize()
    {
      using (ApplicationDbContext applicationDbContext = new ApplicationDbContext())
      {
        applicationDbContext.Database.EnsureDeleted();
        applicationDbContext.Database.Migrate();
      }
    }


    [TestMethod()]
    public async Task T01_GetCoinDepot_CoinTypesCount_ShouldReturn6Types_3perType_SumIs1155Cents()
    {
      var controller = new OrderController(new UnitOfWork());
      var depot = await controller.GetCoinDepotAsync();
      Assert.AreEqual(6, depot.Count(), "Sechs Münzarten im Depot");
      foreach (var coin in depot)
      {
        Assert.AreEqual(3, coin.Amount, "Je Münzart sind drei Stück im Depot");
      }
      int sumOfCents = depot.Sum(coin => coin.CoinValue * coin.Amount);
      Assert.AreEqual(1155, sumOfCents, "Beim Start sind 1155 Cents im Depot");
    }

    [TestMethod()]
    public async Task T02_GetProducts_9Products_FromCappuccinoToRistretto()
    {
      var statisticsController = new OrderController(new UnitOfWork());
      var products = (await statisticsController.GetProductsAsync()).ToArray();
      Assert.AreEqual(9, products.Length, "Neun Produkte wurden erzeugt");
      Assert.AreEqual("Cappuccino", products[0].Name);
      Assert.AreEqual("Ristretto", products[8].Name);
    }

    [TestMethod()]
    public async Task T03_BuyOneCoffee_OneCoinIsEnough_CheckCoinsAndOrders()
    {
      UnitOfWork unitOfWork = new UnitOfWork();
      OrderController controller = new OrderController(unitOfWork);
      var products = await controller.GetProductsAsync();
      var product = products.Single(p => p.Name == "Cappuccino");
      var order = await controller.OrderCoffeeAsync(product);
      bool isFinished = await controller.InsertCoinAsync(order, 100);
      Assert.AreEqual(true, isFinished, "100 Cent genügen");
      Assert.AreEqual(100, order.ThrownInCents, "Einwurf stimmt nicht");
      Assert.AreEqual(100 - product.PriceInCents, order.ReturnCents);
      Assert.AreEqual(0, order.DonationCents);
      Assert.AreEqual("20;10;5", order.ReturnCoinValues);
      // Depot überprüfen
      var coins = await controller.GetCoinDepotAsync();
      int sumOfCents = coins.Sum(c => c.CoinValue * c.Amount);
      Assert.AreEqual(1220, sumOfCents, "Beim Start sind 1155 Cents + 65 Cents für Cappuccino");
      Assert.AreEqual("3*200 + 4*100 + 3*50 + 2*20 + 2*10 + 2*5", await controller.GetCoinDepotStringAsync());
      var orders = (await unitOfWork.Orders.GetAllWithProductAsync()).ToArray();
      Assert.AreEqual(1, orders.Length, "Es ist genau eine Bestellung");
      Assert.AreEqual(0, orders[0].DonationCents, "Keine Spende");
      Assert.AreEqual(100, orders[0].ThrownInCents, "100 Cents wurden eingeworfen");
      Assert.AreEqual("Cappuccino", orders[0].Product.Name, "Produktname Cappuccino");
    }

    [TestMethod()]
    public async Task T04_BuyOneCoffee_ExactTHrowInOneCoin_CheckCoinsAndOrders()
    {
      UnitOfWork unitOfWork = new UnitOfWork();
      OrderController controller = new OrderController(unitOfWork);
      var products = await controller.GetProductsAsync();
      var product = products.Single(p => p.Name == "Latte");
      var order = await controller.OrderCoffeeAsync(product);
      bool isFinished = await controller.InsertCoinAsync(order, 50);
      Assert.AreEqual(true, isFinished, "50 Cent stimmen genau");
      Assert.AreEqual(50, order.ThrownInCents, "Einwurf stimmt nicht");
      Assert.AreEqual(0, order.ReturnCents);
      Assert.AreEqual(0, order.DonationCents);
      Assert.AreEqual("", order.ReturnCoinValues);
      // Depot überprüfen
      var coins = await controller.GetCoinDepotAsync();
      int sumOfCents = coins.Sum(c => c.CoinValue * c.Amount);
      Assert.AreEqual(1205, sumOfCents, "Beim Start sind 1155 Cents + 50 Cents für Latte");
      Assert.AreEqual("3*200 + 3*100 + 4*50 + 3*20 + 3*10 + 3*5", await controller.GetCoinDepotStringAsync());
      var orders = (await unitOfWork.Orders.GetAllWithProductAsync()).ToArray();
      Assert.AreEqual(1, orders.Length, "Es ist genau eine Bestellung");
      Assert.AreEqual(0, orders[0].DonationCents, "Keine Spende");
      Assert.AreEqual(50, orders[0].ThrownInCents, "100 Cents wurden eingeworfen");
      Assert.AreEqual("Latte", orders[0].Product.Name, "Produktname Latte");
    }

    [TestMethod()]
    public async Task T05_BuyOneCoffee_MoreCoins_CheckCoinsAndOrders()
    {
      UnitOfWork unitOfWork = new UnitOfWork();
      OrderController controller = new OrderController(unitOfWork);
      var products = await controller.GetProductsAsync();
      var product = products.Single(p => p.Name == "Cappuccino");
      var order = await controller.OrderCoffeeAsync(product);
      bool isFinished = await controller.InsertCoinAsync(order, 10);
      Assert.AreEqual(false, isFinished, "10 Cent genügen nicht");
      Assert.AreEqual(10, order.ThrownInCents, "Einwurf stimmt nicht");
      Assert.AreEqual("10", order.ThrownInCoinValues);
      isFinished = await controller.InsertCoinAsync(order, 10);
      Assert.AreEqual(false, isFinished, "20 Cent genügen nicht");
      Assert.AreEqual(20, order.ThrownInCents, "Einwurf stimmt nicht");
      Assert.AreEqual("10;10", order.ThrownInCoinValues);
      isFinished = await controller.InsertCoinAsync(order, 20);
      Assert.AreEqual(false, isFinished, "40 Cent genügen nicht");
      Assert.AreEqual(40, order.ThrownInCents, "Einwurf stimmt nicht");
      Assert.AreEqual("10;10;20", order.ThrownInCoinValues);
      isFinished = await controller.InsertCoinAsync(order, 5);
      Assert.AreEqual(false, isFinished, "45 Cent genügen nicht");
      Assert.AreEqual(45, order.ThrownInCents, "Einwurf stimmt nicht");
      Assert.AreEqual("10;10;20;5", order.ThrownInCoinValues);
      Assert.AreEqual("3*200 + 3*100 + 3*50 + 3*20 + 3*10 + 3*5", await controller.GetCoinDepotStringAsync());
      isFinished = await controller.InsertCoinAsync(order, 50);
      Assert.AreEqual(true, isFinished, "95 Cent genügen");
      Assert.AreEqual(95, order.ThrownInCents, "Einwurf stimmt nicht");
      Assert.AreEqual("10;10;20;5;50", order.ThrownInCoinValues);
      Assert.AreEqual(95 - product.PriceInCents, order.ReturnCents);
      Assert.AreEqual(0, order.DonationCents);
      Assert.AreEqual("20;10", order.ReturnCoinValues);
      // Depot überprüfen
      var coins = await controller.GetCoinDepotAsync();
      int sumOfCents = coins.Sum(c => c.CoinValue * c.Amount);
      Assert.AreEqual(1220, sumOfCents, "Beim Start sind 1155 Cents + 65 Cents für Cappuccino");
      Assert.AreEqual("3*200 + 3*100 + 4*50 + 3*20 + 4*10 + 4*5", await controller.GetCoinDepotStringAsync());
    }


    [TestMethod()]
    public async Task T06_BuyMoreCoffees_OneCoins_CheckCoinsAndOrders()
    {
      UnitOfWork unitOfWork = new UnitOfWork();
      OrderController controller = new OrderController(unitOfWork);
      var products = await controller.GetProductsAsync();
      var product = products.Single(p => p.Name == "Cappuccino");
      var order = await controller.OrderCoffeeAsync(product);
      bool isFinished = await controller.InsertCoinAsync(order, 100);
      Assert.IsTrue(isFinished);
      Assert.AreEqual("3*200 + 4*100 + 3*50 + 2*20 + 2*10 + 2*5", await controller.GetCoinDepotStringAsync());
      product = products.Single(p => p.Name == "Latte");
      order = await controller.OrderCoffeeAsync(product);
      isFinished = await controller.InsertCoinAsync(order, 100);
      Assert.IsTrue(isFinished);
      Assert.AreEqual("3*200 + 5*100 + 2*50 + 2*20 + 2*10 + 2*5", await controller.GetCoinDepotStringAsync());
      product = products.Single(p => p.Name == "Cappuccino");
      order = await controller.OrderCoffeeAsync(product);
      isFinished = await controller.InsertCoinAsync(order, 200);
      Assert.IsTrue(isFinished);
      Assert.AreEqual("4*200 + 4*100 + 2*50 + 1*20 + 1*10 + 1*5", await controller.GetCoinDepotStringAsync());
      Assert.AreEqual(0, order.DonationCents);
    }


    [TestMethod()]
    public async Task T07_BuyMoreCoffees_UntilDonation_CheckCoinsAndOrders()
    {
      UnitOfWork unitOfWork = new UnitOfWork();
      OrderController controller = new OrderController(unitOfWork);
      var products = await controller.GetProductsAsync();
      var product = products.Single(p => p.Name == "Cappuccino");
      var order = await controller.OrderCoffeeAsync(product);
      bool isFinished = await controller.InsertCoinAsync(order, 200);
      Assert.IsTrue(isFinished);
      Assert.AreEqual("4*200 + 2*100 + 3*50 + 2*20 + 2*10 + 2*5", await controller.GetCoinDepotStringAsync());
      product = products.Single(p => p.Name == "Cappuccino");
      order = await controller.OrderCoffeeAsync(product);
      isFinished = await controller.InsertCoinAsync(order, 200);
      Assert.IsTrue(isFinished);
      Assert.AreEqual("5*200 + 1*100 + 3*50 + 1*20 + 1*10 + 1*5", await controller.GetCoinDepotStringAsync());
      product = products.Single(p => p.Name == "Cappuccino");
      order = await controller.OrderCoffeeAsync(product);
      isFinished = await controller.InsertCoinAsync(order, 200);
      Assert.IsTrue(isFinished);
      Assert.AreEqual("6*200 + 0*100 + 3*50 + 0*20 + 0*10 + 0*5", await controller.GetCoinDepotStringAsync());
      Assert.AreEqual(0, order.DonationCents);
      product = products.Single(p => p.Name == "Cappuccino");
      order = await controller.OrderCoffeeAsync(product);
      isFinished = await controller.InsertCoinAsync(order, 200);
      Assert.IsTrue(isFinished);
      Assert.AreEqual("7*200 + 0*100 + 1*50 + 0*20 + 0*10 + 0*5", await controller.GetCoinDepotStringAsync());
      Assert.AreEqual(35, order.DonationCents);
    }
  }
}
