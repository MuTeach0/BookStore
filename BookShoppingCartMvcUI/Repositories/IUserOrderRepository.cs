namespace BookShoppingCartMvcUI.Repositories;

public interface IUserOrderRepository
{
    Task<IEnumerable<Order>> UserOrders(bool getAll = false);
    Task ChangeOrerStatus(UpdateOrderStatusModel data);
    Task TogglePaymentStatus(int orderId);
    Task<Order?> GetOrderById(int Id);
    Task<IEnumerable<OrderStatus>> GetOrderStatuses();
}