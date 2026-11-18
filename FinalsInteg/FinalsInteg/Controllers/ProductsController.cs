using FinalsInteg.Data;
using FinalsInteg.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FinalsInteg.Controllers;

[Authorize(Roles = "Admin")]
public class ProductsController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? search) => View(await db.Products.Where(p => string.IsNullOrWhiteSpace(search) || p.Name.Contains(search) || p.Category.Contains(search)).OrderBy(p => p.Name).ToListAsync());

    public IActionResult Create() => View(new Product());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid) return View(product);
        db.Products.Add(product); await db.SaveChangesAsync(); TempData["Success"] = "Product added successfully."; return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id) { var item = await db.Products.FindAsync(id); return item is null ? NotFound() : View(item); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.ProductId) return NotFound();
        if (!ModelState.IsValid) return View(product);
        db.Update(product); await db.SaveChangesAsync(); TempData["Success"] = "Product updated successfully."; return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id) { var item = await db.Products.FindAsync(id); return item is null ? NotFound() : View(item); }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await db.Products.FindAsync(id); if (item is null) return NotFound();
        if (await db.Transactions.AnyAsync(t => t.ProductId == id)) { TempData["Error"] = "This product has transaction history and cannot be deleted."; return RedirectToAction(nameof(Index)); }
        db.Products.Remove(item); await db.SaveChangesAsync(); TempData["Success"] = "Product deleted."; return RedirectToAction(nameof(Index));
    }
}
