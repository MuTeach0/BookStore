using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUI.Repositories;

public class GenreRepository(ApplicationDbContext context) : IGenreRepository
{
    private readonly ApplicationDbContext _context = context;
    
    public async Task AddGenre(Genre genre)
    {
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteGenre(Genre genre)
    {
        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
    }

    public async Task<Genre?> GetGenreById(int id) =>
        await _context.Genres.FindAsync(id);

    public async Task<IEnumerable<Genre>> GetGenres() =>
        await _context.Genres.ToListAsync();

    public async Task UpdateGenre(Genre genre)
    {
        _context.Genres.Update(genre);
         await _context.SaveChangesAsync();
    }
}