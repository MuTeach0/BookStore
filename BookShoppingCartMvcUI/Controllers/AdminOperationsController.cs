using BookShoppingCartMvcUI.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShoppingCartMvcUI.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class AdminOperationsController(IUserOrderRepository userOrderRepository) : Controller
{
    private readonly IUserOrderRepository _userOrderRepository = userOrderRepository;

    public async Task<IActionResult> AllOrders()
    {
        var orders = await _userOrderRepository.UserOrders(true);
        return View(orders);
    }

    public async Task<IActionResult> TogglePaymentStatus(int orderId)
    {
        try
        {
            await _userOrderRepository.TogglePaymentStatus(orderId);
        }
        catch (Exception ex)
        {
            // log exception here
        }
        return RedirectToAction(nameof(AllOrders));
    }

    public async Task<IActionResult> UpdateOrderStatus(int orderId)
    {
        var order = await _userOrderRepository.GetOrderById(orderId) ??
            throw new Exception($"Order with id: {orderId} does not found.");

        var orderStatusList =
            (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus =>
            {
                return new SelectListItem
                {
                    Value = orderStatus.Id.ToString(),
                    Text = orderStatus.StatusName,
                    Selected = order.OrderStatusId == orderStatus.Id
                };
            });

        var data = new UpdateOrderStatusModel
        {
            OrderId = orderId,
            OrderStatusId = order.OrderStatusId,
            OrderStatusList = orderStatusList
        };
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel data)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                data.OrderStatusList = (await _userOrderRepository.GetOrderStatuses())
                    .Select(orderStatus =>
                    {
                        return new SelectListItem
                        {
                            Value = orderStatus.Id.ToString(),
                            Text = orderStatus.StatusName,
                            Selected = orderStatus.Id == data.OrderStatusId
                        };
                    });
                return View(data);
            }
            await _userOrderRepository.ChangeOrerStatus(data);
            TempData["msg"] = "Updated successfully";
        }
        catch (Exception ex)
        {
            // catch exception here
            TempData["msg"] = "Something went wrong";
        }
        return RedirectToAction(nameof(UpdateOrderStatus), new
        { orderId = data.OrderId });
    }

    public IActionResult Dashboard() => View();
}