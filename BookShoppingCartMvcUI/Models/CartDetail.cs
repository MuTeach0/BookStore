﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShoppingCartMvcUI.Models;

[Table("CartDetail")]
public class CartDetail
{
    public int Id { get; set; }
    [Required]
    public int ShoppingCartId { get; set; }
    [Required]
    public int BookId { get; set; }
    [Required]
    public int Quantity { get; set; }
    [Required]
    public decimal UnitPrice { get; set; }
    public ShoppingCart ShoppingCart { get; set; }
    public Book Book { get; set; }
}