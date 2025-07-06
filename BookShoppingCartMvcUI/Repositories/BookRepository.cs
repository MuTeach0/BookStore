
using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUI.Repositories;

public class BookRepository(ApplicationDbContext context) : IBookRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task AddBook(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBook(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Book>> GetAllBooks() =>
        await _context.Books.Include(a => a.Genre).ToListAsync();

    public async Task<Book?> GetBookById(int id) =>
        await _context.Books.FindAsync(id);

    public async Task UpdateBook(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();

    }
}