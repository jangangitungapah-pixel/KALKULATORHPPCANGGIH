using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Globalization;

namespace Hpp.Infrastructure;

/// <summary>
/// EF Core DbContext for HPP.
/// </summary>
public class HppDbContext : DbContext
{
    public HppDbContext(DbContextOptions<HppDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items => Set<Item>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<InventoryLot> InventoryLots => Set<InventoryLot>();
    public DbSet<BillOfMaterials> BillsOfMaterials => Set<BillOfMaterials>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<CalculationRecord> CalculationRecords => Set<CalculationRecord>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var moneyConverter = new ValueConverter<Money, string>(
            v => SerializeMoney(v),
            v => DeserializeMoney(v));

        var quantityConverter = new ValueConverter<Quantity, decimal>(
            v => v.Value,
            v => new Quantity(v));

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasIndex(x => x.Sku).IsUnique();
            entity.Property(x => x.Sku).HasMaxLength(64);
            entity.Property(x => x.Name).HasMaxLength(256);
            entity.Property(x => x.Category).HasMaxLength(128);
            entity.Property(x => x.StandardCost).HasConversion(moneyConverter);
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasIndex(x => x.PurchasedAt);
            entity.Property(x => x.UnitCost).HasConversion(moneyConverter);
            entity.Property(x => x.Quantity).HasConversion(quantityConverter);
            entity.Property(x => x.SupplierName).HasMaxLength(128);
        });

        modelBuilder.Entity<InventoryLot>(entity =>
        {
            entity.HasIndex(x => new { x.ItemId, x.ReceivedAt });
            entity.Property(x => x.UnitCost).HasConversion(moneyConverter);
            entity.Property(x => x.QuantityOnHand).HasConversion(quantityConverter);
        });

        modelBuilder.Entity<CalculationRecord>(entity =>
        {
            entity.Property(x => x.ResultCost).HasConversion(moneyConverter);
            entity.Property(x => x.Strategy).HasMaxLength(128);
        });
    }

    private static string SerializeMoney(Money value)
        => $"{value.Amount.ToString(CultureInfo.InvariantCulture)}|{value.Currency}";

    private static Money DeserializeMoney(string value)
    {
        if (Money.TryParse(value, out var money))
        {
            return money;
        }

        var parts = value.Split('|', StringSplitOptions.TrimEntries);
        if (parts.Length > 0 &&
            decimal.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
        {
            var currency = parts.Length > 1 ? parts[1] : "IDR";
            return new Money(amount, currency);
        }

        return Money.Zero("IDR");
    }
}
