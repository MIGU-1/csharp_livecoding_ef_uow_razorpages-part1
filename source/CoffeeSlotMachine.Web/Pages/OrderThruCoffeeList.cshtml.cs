using CoffeeSlotMachine.Core;
using CoffeeSlotMachine.Core.Contracts;
using CoffeeSlotMachine.Core.DataTransferObjects;
using CoffeeSlotMachine.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public void OnGet()
        {
            ViewData["Message"] = "Welcome to our Coffee Machine";

            OrderCoffeeItems = _unitOfWork
                .Products
                .GetProductDtos();
        }

        public IActionResult OnPostProductSelected(int productId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var orderController = new OrderController(_unitOfWork);

            Product product = _unitOfWork.Products.GetById(productId);
            Order order = orderController.OrderCoffee(product);

            return RedirectToPage(
                "/InsertCoin",
                new { orderId = order.Id }
                );
        }
    }
}