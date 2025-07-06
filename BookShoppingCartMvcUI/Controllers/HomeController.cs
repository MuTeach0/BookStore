using System.Diagnostics;
using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCartMvcUI.Controllers;

public class HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IHomeRepository _homeRepository = homeRepository;

    public async Task<IActionResult> Index(string sterm = "", int genreId = 0)
    {
        var books = await _homeRepository.GetBooks(sterm, genreId);
        var genres = await _homeRepository.Genres();
        var bookModel = new BookDisplayModel
        {
            Books = books,
            Genres = genres,
            STerm = sterm,
            GenreId = genreId
        };

        return View(bookModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
