using FinalsInteg.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalsInteg.Controllers;

public class ReportsController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var transactions = await db.Transactions.Include(t => t.Product).OrderByDescending(t => t.TransactionDate).ToListAsync();
        ViewBag.Revenue = transactions.Sum(t => t.TotalAmount);
        ViewBag.Units = transactions.Sum(t => t.Quantity);
        ViewBag.TopProducts = transactions.GroupBy(t => t.Product?.Name ?? "Unknown").Select(g => new { Name = g.Key, Units = g.Sum(t => t.Quantity), Revenue = g.Sum(t => t.TotalAmount) }).OrderByDescending(x => x.Revenue).Take(5).ToList();
        return View(transactions);
    }
}
