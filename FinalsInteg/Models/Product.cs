using System.ComponentModel.DataAnnotations;

namespace FinalsInteg.Models;

public class Product
{
    public int ProductId { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(50)] public string Category { get; set; } = string.Empty;
    [Range(0.01, 1000000)] public decimal Price { get; set; }
    [Range(0, 100000)] public int StockQuantity { get; set; }
    [StringLength(100)] public string Supplier { get; set; } = string.Empty;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
