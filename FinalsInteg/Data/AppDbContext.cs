using FinalsInteg.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalsInteg.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        builder.Entity<Transaction>().Property(t => t.TotalAmount).HasPrecision(18, 2);
        builder.Entity<Transaction>().HasOne(t => t.Product).WithMany(p => p.Transactions).HasForeignKey(t => t.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Product>().HasData(
            new Product { ProductId = 1, Name = "Wireless Keyboard", Category = "Accessories", Price = 1299, StockQuantity = 18, Supplier = "TechSource" },
            new Product { ProductId = 2, Name = "USB-C Hub", Category = "Accessories", Price = 899, StockQuantity = 7, Supplier = "TechSource" },
            new Product { ProductId = 3, Name = "Laptop Stand", Category = "Office", Price = 1599, StockQuantity = 4, Supplier = "ErgoWorks" },
            new Product { ProductId = 4, Name = "Webcam HD", Category = "Devices", Price = 2199, StockQuantity = 24, Supplier = "VisionPro" });
    }
}
