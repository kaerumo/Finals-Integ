using System.ComponentModel.DataAnnotations;

namespace FinalsInteg.Models;

public class Transaction
{
    public int TransactionId { get; set; }
    [Required, StringLength(100)] public string CustomerName { get; set; } = string.Empty;
    [Required] public int ProductId { get; set; }
    public Product? Product { get; set; }
    [Range(1, 100000)] public int Quantity { get; set; }
    [Range(0.01, 10000000)] public decimal TotalAmount { get; set; }
    [Required, StringLength(30)] public string PaymentMethod { get; set; } = "Cash";
    [DataType(DataType.Date)] public DateTime TransactionDate { get; set; } = DateTime.Today;
}
