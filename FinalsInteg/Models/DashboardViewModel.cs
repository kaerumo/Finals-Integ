namespace FinalsInteg.Models;

public class DashboardViewModel
{
    public int TotalProducts { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalRevenue { get; set; }
    public int LowStockCount { get; set; }
    public List<Product> LowStockProducts { get; set; } = new();
    public List<Transaction> RecentTransactions { get; set; } = new();
}
