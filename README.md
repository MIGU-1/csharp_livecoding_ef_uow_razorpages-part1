# LiveCoding Razor Pages

![.NET Core](https://github.com/jfuerlinger/csharp_livecoding_ef_uow_razorpages-part1/workflows/.NET%20Core/badge.svg)

## Layout anpassen => Link hinzufügen

```html
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-page="/OrderThruCoffeeList">Order</a>
</li>
```

## Bestellseite (simple)

## PageModel

```cs
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
```

## View

```html
@page
@model CoffeeSlotMachine.Web.Pages.OrderThruCoffeeListModel
@{
}

<h2>@ViewData["Message"]</h2>

<form method="post">

    <table class="table">

        <thead>
            <tr>
                <th>
                    @Html.DisplayNameFor(model => model.OrderCoffeeItems[0].ProductName)
                </th>
                <th>
                    @Html.DisplayNameFor(model => model.OrderCoffeeItems[0].PriceInCents)
                </th>
                <th>
                    @Html.DisplayNameFor(model => model.OrderCoffeeItems[0].NrOfOrders)
                </th>
                <th></th>
            </tr>
        </thead>
        <tbody>
            @foreach (var item in Model.OrderCoffeeItems)
            {
                <tr>
                    <td>
                        @Html.DisplayFor(_ => item.ProductName)
                    </td>
                    <td>
                        @Html.DisplayFor(_ => item.PriceInCents)
                    </td>
                    <td>
                        @Html.DisplayFor(_ => item.NrOfOrders)
                    </td>
                    <td>
                        <button type="submit"
                                class="btn btn-primary"
                                asp-page-handler="ProductSelected"
                                asp-route-productId="@item.ProductId">
                            Select Product
                        </button>
                    </td>
                </tr>
            }
        </tbody>

    </table>

</form>
```

## Münzeinwurf

## PageModel

```cs
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

    public async Task OnGet(int orderId)
    {
        ViewData["Message"] = "Insert Coin";

        Coins = await _unitOfWork.Coins.GetAllAsync();
        OrderId = orderId;
        ActualOrder = await _unitOfWork.Orders.GetByIdWithProductAndCoinsAsync(orderId);
    }

    public async Task<IActionResult> OnPostInsertCoin()
    {
        if (!ModelState.IsValid)
        {
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
```

## View

```html
@page

@model CoffeeSlotMachine.Web.Pages.InsertCoinModel
@{

}

<h2>@ViewData["Message"]</h2>

<div asp-validation-summary="All"></div>

<form method="post">

    <input asp-for="@Model.OrderId" type="hidden" />

    <div class="form-group">
        <label asp-for="@Model.ActualOrder.Product.Name">Product:</label>
        <input asp-for="@Model.ActualOrder.Product.Name" class="form-control" readonly="readonly" />
    </div>

    <div class="form-group">
        <label asp-for="@Model.ActualOrder.Product.PriceInCents">Price:</label>
        <input asp-for="@Model.ActualOrder.Product.PriceInCents" class="form-control" readonly="readonly" />
    </div>

    <div class="form-group">
        <label asp-for="@Model.ActualOrder.ThrownInCents">Amount:</label>
        <input asp-for="@Model.ActualOrder.ThrownInCents" class="form-control" readonly="readonly" />
    </div>


    <div class="form-group">
        <label asp-for="@Model.Coins">Coins:</label>
        <div>
            <select asp-for="@Model.InsertedCoinId" class="form-control">
                @foreach (var coinType in @Model.Coins)
                {
                    <option value="@coinType.Id">@coinType.CoinValue</option>
                }
            </select>
        </div>

    </div>

    <button type="submit"
            class="btn btn-primary"
            asp-page-handler="InsertCoin">
        Insert Coin
    </button>

</form>
```

## Bestellbestätigung

## PageModel

```cs
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

    public async Task OnGet(int orderId)
    {
        ViewData["Message"] = "Enjoy your Coffee";

        OrderController orderController = new OrderController(_unitOfWork);

        ActualOrder = await _unitOfWork.Orders.GetByIdWithProductAndCoinsAsync(orderId);
        CoinDepotString = await orderController.GetCoinDepotStringAsync();
    }

    public IActionResult OnPostNextCoffee()
    {
        return RedirectToPage("/OrderThruCoffeeList");
    }


}
```

## View

```html
@page
@model CoffeeSlotMachine.Web.Pages.CoffeeReadyModel
@{
    ViewData["Title"] = "Coffee Ready";
}

<h2>@ViewData["Message"]</h2>

<form method="post">
    <div class="form-group">
        <label asp-for="@Model.ActualOrder.Product.Name">ProductName:</label>
        <input asp-for="@Model.ActualOrder.Product.Name" class="form-control"
               readonly="readonly" />
    </div>

    <div class="form-group">
        <label asp-for="@Model.ActualOrder.Product.PriceInCents">Price:</label>
        <input asp-for="@Model.ActualOrder.Product.PriceInCents" class="form-control" readonly="readonly" />
    </div>
    <div class="form-group">
        <label asp-for="@Model.ActualOrder.ThrownInCents">ThrownInCents:</label>
        <input asp-for="@Model.ActualOrder.ThrownInCents" class="form-control" readonly="readonly" />
    </div>
    <div class="form-group">
        <label asp-for="@Model.ActualOrder.ReturnCents">ReturnCents:</label>
        <input asp-for="@Model.ActualOrder.ReturnCents" class="form-control" readonly="readonly" />
    </div>
    <div class="form-group">
        <label asp-for="@Model.ReturnCoinsString">Return Coins:</label>
        <input asp-for="@Model.ReturnCoinsString" class="form-control" readonly="readonly" />
    </div>
    <div class="form-group">
        <label asp-for="@Model.CoinDepotString">CoinDepot:</label>
        <input asp-for="@Model.CoinDepotString" class="form-control" readonly="readonly" />
    </div>

    @if (@Model.ActualOrder.DonationCents > 0)
    {
        <div class="form-group">
            <label asp-for="@Model.ActualOrder.DonationCents">Donation Cents:</label>
            <input asp-for="@Model.ActualOrder.DonationCents" class="form-control"
                   readonly="readonly" />
        </div>
    }

    <div class="form-group">
        <label>Thank you for your purchase!</label>
    </div>

    <div class="form-group">
        <button type="submit"
                class="btn btn-primary"
                asp-page-handler="NextCoffee">
            Next Coffee
        </button>
    </div>
</form>
```

## Bestellseite (mit Combobox)

## PageModel

```cs
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
```

## View

```html
@page
@model CoffeeSlotMachine.Web.Pages.OrderThruCoffeeSelectionControlModel
@{
    ViewData["Title"] = "Coffee Slot Home page";
}


<h2>@ViewData["Message"]</h2>

<div asp-validation-summary="All"></div>

<form method="post">
    <div class="form-group">
        Product:
        <select asp-for="@Model.ProductId">
            @foreach (var product in @Model.Products)
            {
                <option value=@product.Id>
                    @(string.Format($"{product.Name,-25} {product.PriceInCents} ct").Replace(" ", "\xA0"))
                </option>
            }
        </select>
    </div>
    <button type="submit"
            class="btn btn-primary"
            asp-page-handler="ProductSelected">
        Select product
    </button>
</form>
```