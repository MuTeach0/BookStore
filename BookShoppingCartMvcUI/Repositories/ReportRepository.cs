using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUI.Repositories;

public class ReportRepository(ApplicationDbContext context) : IReportRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<TopNSoldBookModel>> GetTopNSellingBooksByDate(
        DateTime startDate, DateTime endDate)
    {
        var startDateParam = new SqlParameter("@startDate", startDate);
        var endDateParam = new SqlParameter("@endDate", endDate);

        var topFiveSoldBooks = await _context.Database.SqlQueryRaw<TopNSoldBookModel>(
            "EXEC Usp_GetTopNSellingBooksByDate @startDate, @endDate",
            startDateParam, endDateParam).ToListAsync();

        return topFiveSoldBooks;
    }
}