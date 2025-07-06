using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCartMvcUI.Controllers;

[Authorize]
public class UserOrderController(IUserOrderRepository userOrderRepo) : Controller
{
    private readonly IUserOrderRepository _userOrderRepo = userOrderRepo;

    public async Task<IActionResult> UserOrdersAsync()
    {
        var orders = await _userOrderRepo.UserOrders();
        return View(orders);
    }
}
