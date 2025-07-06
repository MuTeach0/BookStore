using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookShoppingCartMvcUI.Repositories;

public class UserOrderRepository(ApplicationDbContext context,
    UserManager<IdentityUser> userManager,
    IHttpContextAccessor httpContextAccessor) : IUserOrderRepository
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

    public async Task ChangeOrerStatus(UpdateOrderStatusModel data)
    {
        var order = await _context.Orders.FindAsync(data.OrderId) ??
            throw new Exception($"order with id: {data.OrderId} does not found");

        order.OrderStatusId = data.OrderStatusId;
        await _context.SaveChangesAsync();
    }

    public async Task<Order?> GetOrderById(int Id) =>
        await _context.Orders.FindAsync(Id);

    public async Task<IEnumerable<OrderStatus>> GetOrderStatuses() =>
        await _context.orderStatuses.ToListAsync();

    public async Task TogglePaymentStatus(int orderId)
    {
        var order = await GetOrderById(orderId) ??
            throw new Exception($"order with id: {orderId} does not found");
        order.IsPaid = !order.IsPaid;
        await _context.SaveChangesAsync();

        //var order = await _context.Orders.FindAsync(orderId) ??
        //    throw new Exception($"order with id: {orderId} does not found");

        //order.IsPaid = !order.IsPaid;
        //await _context.SaveChangesAsync();

    }

    public async Task<IEnumerable<Order>> UserOrders(bool getAll = false)
    {
        var orders = _context.Orders
                          .Include(x => x.OrderStatus)
                          .Include(x => x.OrderDetails)
                          .ThenInclude(x => x.Book)
                          .ThenInclude(x => x.Genre).AsQueryable();
        if (!getAll)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User is not logged-in");
            orders = orders.Where(a => a.UserId == userId);
            return await orders.ToListAsync();

        }
        return await orders.ToListAsync();
    }

}
//public async Task<IEnumerable<Order>> UserOrders()
//{
//    var userId = GetUserId();
//    if (string.IsNullOrEmpty(userId))
//        throw new Exception("User is not logged-in");
//    var orders = await _context.Orders
//                          .Include(x =>x.OrderStatus)
//                          .Include(x => x.OrderDetails)
//                          .ThenInclude(x => x.Book)
//                          .ThenInclude(x => x.Genre)
//                          .Where(a => a.UserId == userId)
//                          .ToListAsync();
//    return orders;
//}