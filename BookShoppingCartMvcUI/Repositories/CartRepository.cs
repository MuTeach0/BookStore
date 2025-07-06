using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUI.Repositories;

public class CartRepository(ApplicationDbContext context,
    UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor) : ICartRepository
{
    #region Private prop && Method
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private string GetUserId()
    {
        var principal = _httpContextAccessor.HttpContext.User;
        var userId = _userManager.GetUserId(principal);
        return userId;
    }
    #endregion

    public async Task<int> AddItem(int bookId, int qty)
    {
        // cart - saved
        // cartDetail -> error
        string userId = GetUserId();
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("user is not logged-in");

            var cart = await GetCart(userId);

            if (cart is null)
            {
                cart = new ShoppingCart { UserId = userId };
                _context.ShoppingCarts.Add(cart);
            }
            _context.SaveChanges();
            // cart details section
            var cartItem = _context.CartDetails
                .FirstOrDefault(c => c.ShoppingCartId == cart.Id && c.BookId == bookId);
            if (cartItem is not null)
                cartItem.Quantity += qty;
            else
            {
                var book = _context.Books.Find(bookId);
                cartItem = new CartDetail
                {
                    BookId = bookId,
                    ShoppingCartId = cart.Id,
                    UnitPrice = book.Price, // it is a new line after update
                    Quantity = qty

                };
                _context.CartDetails.Add(cartItem);
            }
            _context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception ex)
        {
        }
        var cartItemCount = await GetCartItemCount(userId);
        return cartItemCount;
    }

    public async Task<int> RemoveItem(int bookId)
    {
        // cart - saved
        // cartDetail -> error
        string userId = GetUserId();
        try
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("user is not logged-in");

            var cart = await GetCart(userId) ?? throw new InvalidOperationException("Cart is empty");

            // cart details section
            var cartItem = _context.CartDetails
                .FirstOrDefault(c => c.ShoppingCartId == cart.Id && c.BookId == bookId);

            if (cartItem is null)
                throw new InvalidOperationException("Not item in cart");

            else if (cartItem.Quantity == 1)
                _context.CartDetails.Remove(cartItem);
            else
                cartItem.Quantity--;
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
        }
        var cartItemCount = await GetCartItemCount(userId);
        return cartItemCount;
    }

    public async Task<ShoppingCart> GetUserCart()
    {
        var userId = GetUserId() ?? throw new InvalidOperationException("Invalid userId");

        var shoppingCart = await _context.ShoppingCarts
                          .Include(a => a.CartDetails)
                          .ThenInclude(a => a.Book)
                          .ThenInclude(a => a.Stock)
                          .Include(a => a.CartDetails)
                          .ThenInclude(a => a.Book)
                          .ThenInclude(a => a.Genre)
                          .Where(a => a.UserId == userId).FirstOrDefaultAsync();
        return shoppingCart;
    }

    public async Task<ShoppingCart?> GetCart(string userId)
    {
        var cart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
        return cart;
    }

    public async Task<bool> DoCheckout(CheckoutModel model)
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            // logic
            // move data from cartDetail to order and order detail then we will remove cart detail  

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User is not logged-in");

            var cart = await GetCart(userId) ?? throw new InvalidOperationException("Invalid cart");

            var cartDetail = _context.CartDetails
                            .Where(a => a.ShoppingCartId == cart.Id).ToList();
            if (cartDetail.Count == 0)
                throw new InvalidOperationException("Cart is empty");

            var pendingRecord = _context.orderStatuses.FirstOrDefault
                (s => s.StatusName == "Pending") ??
                throw new InvalidOperationException("Order status does not have Pending status");

            var order = new Order
            {
                UserId = userId,
                CreateDate = DateTime.UtcNow,
                Name = model.Name,
                Email = model.Email,
                MobileNamber = model.MobileNumber,
                PymentMethod = model.PeymentMethod,
                Address = model.Address,
                IsPaid = false,
                OrderStatusId = pendingRecord.Id
            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            foreach (var item in cartDetail)
            {
                var orderDetail = new OrderDetail
                {
                    BookId = item.BookId,
                    OrderId = order.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };
                _context.OrderDetails.Add(orderDetail);

                // update stock here
                var stock = await _context.Stocks.FirstOrDefaultAsync(a => a.BookId == item.BookId) ??
                    throw new InvalidOperationException("Stock is null");
                if (item.Quantity > stock.Quantity)
                    throw new InvalidOperationException(
                        $"Only {stock.Quantity} item(s) are available in the stock");

                /* 
                 * if this book have an entry in the stock table,
                     then update the quantity only.
                 * decrease the number of quantity from the stock table
                 */

                stock.Quantity -=item.Quantity;

            }
            _context.SaveChanges();

            // removing cartdetatils
            _context.CartDetails.RemoveRange(cartDetail);
            _context.SaveChanges();
            transaction.Commit();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<int> GetCartItemCount(string userId = "")
    {
        if (string.IsNullOrEmpty(userId)) // updated line
            userId = GetUserId();

        var data = await (from cart in _context.ShoppingCarts
                          join cartDetail in _context.CartDetails
                          on cart.Id equals cartDetail.ShoppingCartId
                          where cart.UserId == userId // updated line
                          select new { cartDetail.Id }).ToListAsync();
        return data.Count;
    }
}