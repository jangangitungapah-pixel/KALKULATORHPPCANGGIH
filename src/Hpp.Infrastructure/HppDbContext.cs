using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        modelBuilder.Entity<Item>().Property(x => x.StandardCost).HasConversion(moneyConverter);
        modelBuilder.Entity<Purchase>().Property(x => x.UnitCost).HasConversion(moneyConverter);
        modelBuilder.Entity<Purchase>().Property(x => x.Quantity).HasConversion(quantityConverter);
        modelBuilder.Entity<InventoryLot>().Property(x => x.UnitCost).HasConversion(moneyConverter);
        modelBuilder.Entity<InventoryLot>().Property(x => x.QuantityOnHand).HasConversion(quantityConverter);
        modelBuilder.Entity<CalculationRecord>().Property(x => x.ResultCost).HasConversion(moneyConverter);
    }

    private static string SerializeMoney(Money value) => $"{value.Amount}|{value.Currency}";

    private static Money DeserializeMoney(string value)
    {
        var parts = value.Split('|');
        return new Money(decimal.Parse(parts[0]), parts.Length > 1 ? parts[1] : "IDR");
    }
}
