using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeSlotMachine.Core;
using CoffeeSlotMachine.Core.Contracts;
using CoffeeSlotMachine.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeSlotMachine.Web.Pages
{
  public class CoffeeReadyModel : PageModel
  {

    private readonly IUnitOfWork _unitOfWork;

    public string CoinDepotString { get; set; }

    public Order ActualOrder { get; set; }

    public string ReturnCoinsString
    {
      get
      {
        if (String.IsNullOrEmpty(ActualOrder.ReturnCoinValues))
        {
          return "-";
        }
        StringBuilder coinsString = new StringBuilder();
        int count = 1;
        string[] centsStrings = ActualOrder.ReturnCoinValues.Split(";");
        for (int i = 1; i < centsStrings.Length; i++)
        {
          if (centsStrings[i] == centsStrings[i - 1])
          {
            count++;
          }
          else
          {
            coinsString.Append($"{count}*{centsStrings[i - 1]} + ");
            count = 1;
          }
        }
        coinsString.Append($"{count}*{centsStrings[centsStrings.Length - 1]} ");
        return coinsString.ToString();
      }
    }

    public CoffeeReadyModel(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }


    public async Task OnGet(int orderId)
    {
      ViewData["Message"] = "Enjoy your Coffee";

      OrderController orderController = new OrderController(_unitOfWork);

      ActualOrder = await _unitOfWork.Orders.GetByIdWithProductAndCoinsAsync(orderId);
      CoinDepotString = await orderController.GetCoinDepotStringAsync();
    }

    public IActionResult OnPostNextCoffee()
    {
      return RedirectToPage("/OrderCoffeeThruList");
    }
  }
}