using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    public bool InStock { get; set; } = true;
}