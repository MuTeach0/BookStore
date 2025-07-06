using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShoppingCartMvcUI.Models;

[Table("Book")]
public class Book
{
    public int Id { get; set; }
    [Required]
    [MaxLength(40)]
    public string? BookName { get; set; }
    [Required]
    [MaxLength(40)]
    public string? AuthorName { get; set; }
    [Required]
    public decimal Price { get; set; }
    public string? Image { get; set; }
    [Required]
    public int GenreId { get; set; }

    // النوع
    public Genre Genre { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }
    public List<CartDetail> CartDetails { get; set; }
    public Stock Stock { get; set; }
    [NotMapped]
    public string GenreName { get; set; }
    [NotMapped]
    public int Quantity { get; set; }
}