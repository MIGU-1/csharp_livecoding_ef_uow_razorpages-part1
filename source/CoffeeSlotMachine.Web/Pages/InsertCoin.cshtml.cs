using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CoffeeSlotMachine.Core;
using CoffeeSlotMachine.Core.Contracts;
using CoffeeSlotMachine.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeSlotMachine.Web.Pages
{
  public class InsertCoinModel : PageModel
  {
    private readonly IUnitOfWork _unitOfWork;

    [BindProperty]
    public int OrderId { get; set; }

    [Range(1, 15, ErrorMessage = "Ungültige Münze")]
    [BindProperty]
    public int InsertedCoinId { get; set; }


    public Order ActualOrder { get; set; }
    public IEnumerable<Coin> Coins { get; set; }

    public InsertCoinModel(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    public async Task OnGet(int orderId)
    {
      ViewData["Message"] = "Insert Coin";

      Coins = await _unitOfWork.Coins.GetAllAsync();
      OrderId = orderId;
      ActualOrder = await _unitOfWork.Orders.GetByIdWithProductAndCoinsAsync(orderId);
    }

    public async Task<IActionResult> OnPostCoinInserted()
    {
      if (!ModelState.IsValid)
      {
        Coins = await _unitOfWork.Coins.GetAllAsync();
        return Page();
      }


      OrderController orderController = new OrderController(_unitOfWork);
      var order = await _unitOfWork.Orders.GetByIdWithProductAndCoinsAsync(OrderId);

      Coins = await _unitOfWork.Coins.GetAllAsync();
      var coin = Coins.Single(ct => ct.Id == InsertedCoinId);
      
      if (await orderController.InsertCoinAsync(order, coin.CoinValue))
      {
        return RedirectToPage(
            "/CoffeeReady",
            new { orderId = order.Id });
      }

      ViewData["Message"] = "Insert another Coin";

      ActualOrder = order;

      return Page();
    }

  }
}