using FinalsInteg.Data;
using FinalsInteg.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FinalsInteg.Controllers;

public class HomeController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var transactions = await db.Transactions.Include(t => t.Product).OrderByDescending(t => t.TransactionDate).Take(5).ToListAsync();
        var lowStock = await db.Products.Where(p => p.StockQuantity <= 10).OrderBy(p => p.StockQuantity).ToListAsync();
        return View(new DashboardViewModel { TotalProducts = await db.Products.CountAsync(), TotalTransactions = await db.Transactions.CountAsync(), TotalRevenue = await db.Transactions.SumAsync(t => (decimal?)t.TotalAmount) ?? 0, LowStockCount = lowStock.Count, LowStockProducts = lowStock, RecentTransactions = transactions });
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
