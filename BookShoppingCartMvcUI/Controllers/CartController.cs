using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookShoppingCartMvcUI.Controllers;

[Authorize]
public class CartController(ICartRepository cartRepo) : Controller
{
    private readonly ICartRepository _cartRepo = cartRepo;

    public async Task<IActionResult> AddItem(int bookId, int qty = 1, int redirect = 0)
    {
        var cartCount = await _cartRepo.AddItem(bookId, qty);
        if (redirect == 0)
            return Ok(cartCount);
        return RedirectToAction("GetUserCart");
    }

    public async Task<IActionResult> RemoveItemAsync(int bookId)
    {
        _ = await _cartRepo.RemoveItem(bookId);
        return RedirectToAction("GetUserCart");
    }

    public async Task<IActionResult> GetUserCart()
    {
        var cart = await _cartRepo.GetUserCart();
        return View(cart);
    }

    public async Task<IActionResult> GetTotalItemInCart()
    {
        int cartItem = await _cartRepo.GetCartItemCount();
        return Ok(cartItem);
    }

    public IActionResult Checkout() => View();

    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var isCheckedOut = await _cartRepo.DoCheckout(model);
        if (!isCheckedOut)
            return RedirectToAction(nameof(OrderFailure));
        return RedirectToAction(nameof(OrderSuccess));
    }

    public IActionResult OrderSuccess() => View();
    public IActionResult OrderFailure() => View();

}