using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCartMvcUI.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class ReportsController(IReportRepository reportRepo) : Controller
{
    private readonly IReportRepository _reportRepo = reportRepo;

    // Get: ReportController
    public async Task<IActionResult> TopFiveSellingBooks(DateTime? sDate = null,
        DateTime? eDate = null)
    {
        try
        {
            // by default, get last 7 days record
            DateTime startDate = sDate ?? DateTime.UtcNow.AddDays(-7);
            DateTime endDate = eDate ?? DateTime.UtcNow;
            var topFiveSellingBooks = await _reportRepo.GetTopNSellingBooksByDate
                (startDate, endDate);
            var vm = new TopNSoldBooksVm(startDate, endDate, topFiveSellingBooks);
            return View(vm);
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = "Something went wrong";
            return RedirectToAction("Index", "Home");
        }
    }
}
