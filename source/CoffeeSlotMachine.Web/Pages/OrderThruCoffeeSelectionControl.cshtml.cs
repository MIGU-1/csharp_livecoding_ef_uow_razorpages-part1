using CoffeeSlotMachine.Core;
using CoffeeSlotMachine.Core.Contracts;
using CoffeeSlotMachine.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeSlotMachine.Web.Pages
{
  public class OrderThruCoffeeSelectionControlModel : PageModel
  {
    private readonly IUnitOfWork _unitOfWork;

    public OrderThruCoffeeSelectionControlModel(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }


    [BindProperty]
    public int ProductId { get; set; }

    public IEnumerable<Product> Products { get; set; }


    /// <summary>
    /// Startseite wird zur Auswahl der Kaffeesorte verwendet
    /// </summary>
    /// <returns></returns>
    public async Task OnGet()
    {
      ViewData["Message"] = "Welcome to our Coffee Machine implemented with Razor Pages!";
      OrderController orderController = new OrderController(_unitOfWork);
      Products = await orderController.GetProductsAsync();
    }

    /// <summary>
    /// Auswahl des Kaffetyps und Anlegen einer Bestellung
    /// </summary>
    public async Task<IActionResult> OnPostProductSelected()
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }


      var orderController = new OrderController(_unitOfWork);

      Product product = await _unitOfWork.Products.GetByIdAsync(ProductId);
      Order order = await orderController.OrderCoffeeAsync(product);

      return RedirectToPage(
          "/InsertCoin",
          new { orderId = order.Id });
    }
  }
}