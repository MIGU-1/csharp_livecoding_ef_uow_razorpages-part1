using CoffeeSlotMachine.Core;
using CoffeeSlotMachine.Core.Contracts;
using CoffeeSlotMachine.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CoffeeSlotMachine.Web.Pages
{
    public class InsertCoinModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public Order ActualOrder { get; set; }

        [BindProperty]
        public int OrderId { get; set; }

        public IEnumerable<Coin> Coins { get; set; }

        [Range(1, 15)]
        [BindProperty]
        public int InsertedCoinId { get; set; }

        public InsertCoinModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void OnGet(int orderId)
        {
            ViewData["Message"] = "Insert Coin";

            Coins = _unitOfWork.Coins.GetAll();
            OrderId = orderId;
            ActualOrder = _unitOfWork.Orders.GetByIdWithProductAndCoins(orderId);
        }

        public IActionResult OnPostInsertCoin()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            OrderController orderController = new OrderController(_unitOfWork);
            var order = _unitOfWork.Orders.GetByIdWithProductAndCoins(OrderId);

            Coins = _unitOfWork.Coins.GetAll();
            var coin = Coins.Single(ct => ct.Id == InsertedCoinId);

            if (orderController.InsertCoin(order, coin.CoinValue))
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