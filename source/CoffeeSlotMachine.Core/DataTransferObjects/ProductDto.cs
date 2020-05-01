using System.ComponentModel;

namespace CoffeeSlotMachine.Core.DataTransferObjects
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        [DisplayName("Product")]
        public string ProductName { get; set; }

        [DisplayName("Price")]
        public int PriceInCents { get; set; }

        [DisplayName("Orders")]
        public int NrOfOrders { get; set; }
    }
}
