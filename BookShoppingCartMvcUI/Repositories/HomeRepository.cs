using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUI.Repositories;

public class HomeRepository(ApplicationDbContext context) : IHomeRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task<IEnumerable<Genre>> Genres() =>
        await _context.Genres.ToListAsync();

    public async Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0)
    {
        sTerm = sTerm.ToLower();
        var books = await (from book in _context.Books
                           join genre in _context.Genres
                           on book.GenreId equals genre.Id
                           join stock in _context.Stocks
                           on book.Id equals stock.BookId
                           into book_stock
                           from bookWithStock in book_stock.DefaultIfEmpty()
                           where string.IsNullOrWhiteSpace(sTerm) || (book != null && book.BookName.ToLower().StartsWith(sTerm))
                           select new Book
                           {
                               Id = book.Id,
                               Image = book.Image,
                               AuthorName = book.AuthorName,
                               BookName = book.BookName,
                               GenreId = book.GenreId,
                               Price = book.Price,
                               GenreName = genre.GenreName,
                               Quantity = bookWithStock == null ? 0 : bookWithStock.Quantity

                           }).ToListAsync();
        if (genreId > 0)
            books = [.. books.Where(a => a.GenreId == genreId)];
        return books;
    }
}