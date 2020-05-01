using CoffeeSlotMachine.Core;
using CoffeeSlotMachine.Core.Contracts;
using CoffeeSlotMachine.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Text;

namespace CoffeeSlotMachine.Web.Pages
{
    public class CoffeeReadyModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public string CoinDepotString { get; set; }

        public Order ActualOrder { get; set; }

        public CoffeeReadyModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

        public void OnGet(int orderId)
        {
            ViewData["Message"] = "Enjoy your Coffee";

            OrderController orderController = new OrderController(_unitOfWork);

            ActualOrder = _unitOfWork.Orders.GetByIdWithProductAndCoins(orderId);
            CoinDepotString = orderController.GetCoinDepotString();
        }

        public IActionResult OnPostNextCoffee()
        {
            return RedirectToPage("/OrderThruCoffeeList");
        }


    }
}