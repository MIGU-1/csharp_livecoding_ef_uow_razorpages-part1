using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoffeeSlotMachine.Core;
using CoffeeSlotMachine.Core.Contracts;
using CoffeeSlotMachine.Core.DataTransferObjects;
using CoffeeSlotMachine.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeSlotMachine.Web.Pages
{
  public class OrderCoffeeThruListModel : PageModel
  {
    private readonly IUnitOfWork _unitOfWork;

    public ProductDto[] OrderCoffeeItems { get; set; }

    public OrderCoffeeThruListModel(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    public async Task OnGet()
    {
      ViewData["Message"] = "Welcome to our coffee machine!";
      OrderCoffeeItems = await _unitOfWork.Products.GetProductDtosAsync();
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