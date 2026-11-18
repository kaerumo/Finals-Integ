using FinalsInteg.Data;
using FinalsInteg.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalsInteg.Controllers;

[Authorize(Roles = "Customer")]
public class CustomerController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? search)
    {
        var products = await db.Products
            .Where(p => p.StockQuantity > 0)
            .Where(p => string.IsNullOrWhiteSpace(search) || p.Name.Contains(search) || p.Category.Contains(search))
            .OrderBy(p => p.Name)
            .ToListAsync();

        ViewBag.Search = search;
        return View(products);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Purchase(int productId, int quantity, string customerName)
    {
        var product = await db.Products.FindAsync(productId);
        if (product is null) return NotFound();
        if (string.IsNullOrWhiteSpace(customerName))
        {
            TempData["Error"] = "Customer name is required.";
            return RedirectToAction(nameof(Index));
        }
        if (quantity < 1 || quantity > product.StockQuantity)
        {
            TempData["Error"] = "Please select a valid quantity.";
            return RedirectToAction(nameof(Index));
        }

        product.StockQuantity -= quantity;
        db.Transactions.Add(new Transaction
        {
            CustomerName = customerName.Trim(),
            ProductId = product.ProductId,
            Quantity = quantity,
            TotalAmount = product.Price * quantity,
            PaymentMethod = "Cash",
            TransactionDate = DateTime.Today
        });
        await db.SaveChangesAsync();

        TempData["Success"] = $"Purchase completed for {quantity} x {product.Name}.";
        return RedirectToAction(nameof(Index));
    }
}
