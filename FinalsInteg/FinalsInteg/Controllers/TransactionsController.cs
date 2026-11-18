using FinalsInteg.Data;
using FinalsInteg.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FinalsInteg.Controllers;

[Authorize(Roles = "Admin")]
public class TransactionsController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? search) => View(await db.Transactions.Include(t => t.Product).Where(t => string.IsNullOrWhiteSpace(search) || t.CustomerName.Contains(search) || (t.Product != null && t.Product.Name.Contains(search))).OrderByDescending(t => t.TransactionDate).ToListAsync());

    public async Task<IActionResult> Create() { ViewBag.Products = await db.Products.OrderBy(p => p.Name).ToListAsync(); return View(new Transaction()); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Transaction transaction)
    {
        var product = await db.Products.FindAsync(transaction.ProductId);
        if (product is null) ModelState.AddModelError("ProductId", "Select a valid product.");
        else if (product.StockQuantity < transaction.Quantity) ModelState.AddModelError("Quantity", "Quantity exceeds available stock.");
        if (!ModelState.IsValid) { ViewBag.Products = await db.Products.OrderBy(p => p.Name).ToListAsync(); return View(transaction); }
        transaction.TotalAmount = product!.Price * transaction.Quantity; product.StockQuantity -= transaction.Quantity;
        db.Transactions.Add(transaction); await db.SaveChangesAsync(); TempData["Success"] = "Sale recorded and inventory updated."; return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id) { var item = await db.Transactions.FindAsync(id); if (item is null) return NotFound(); ViewBag.Products = await db.Products.OrderBy(p => p.Name).ToListAsync(); return View(item); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Transaction transaction)
    {
        if (id != transaction.TransactionId) return NotFound();
        var product = await db.Products.FindAsync(transaction.ProductId); if (product is null) ModelState.AddModelError("ProductId", "Select a valid product.");
        if (!ModelState.IsValid) { ViewBag.Products = await db.Products.OrderBy(p => p.Name).ToListAsync(); return View(transaction); }
        transaction.TotalAmount = product!.Price * transaction.Quantity; db.Update(transaction); await db.SaveChangesAsync(); TempData["Success"] = "Transaction updated."; return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id) { var item = await db.Transactions.FindAsync(id); if (item is null) return NotFound(); db.Transactions.Remove(item); await db.SaveChangesAsync(); TempData["Success"] = "Transaction deleted."; return RedirectToAction(nameof(Index)); }
}
