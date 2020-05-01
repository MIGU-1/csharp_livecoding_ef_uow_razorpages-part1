using CoffeeSlotMachine.Core;
using CoffeeSlotMachine.Core.Contracts;
using CoffeeSlotMachine.Core.DataTransferObjects;
using CoffeeSlotMachine.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CoffeeSlotMachine.Web.Pages
{
  public class OrderThruCoffeeListModel : PageModel
  {
    private readonly IUnitOfWork _unitOfWork;

    public ProductDto[] OrderCoffeeItems { get; set; }

    public OrderThruCoffeeListModel(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    public async Task OnGet()
    {
      ViewData["Message"] = "Welcome to our Coffee Machine";

      OrderCoffeeItems = await _unitOfWork
          .Products
          .GetProductDtosAsync();
    }

    public async Task<IActionResult> OnPostProductSelected(int productId)
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }

      var orderController = new OrderController(_unitOfWork);

      Product product = await _unitOfWork.Products.GetByIdAsync(productId);
      Order order = await orderController.OrderCoffeeAsync(product);

      return RedirectToPage(
          "/InsertCoin",
          new { orderId = order.Id }
          );
    }
  }
}