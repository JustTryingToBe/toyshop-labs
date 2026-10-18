using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToyShop.Core;

namespace ToyShop.Web.Pages;

public class OrderModel : PageModel
{
    private readonly OrderService _orders;

    public OrderModel(OrderService orders)
    {
        _orders = orders;
    }

    public Order Order { get; private set; } = null!;

    public IActionResult OnGet(int id)
    {
        var order = _orders.Find(id);
        if (order is null)
        {
            return NotFound();
        }

        Order = order;
        return Page();
    }
}
