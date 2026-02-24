namespace MultiDomainTradingEngine.Data;

using Microsoft.EntityFrameworkCore;
using MultiDomainTradingEngine.Entities;


/// <summary>
/// The primary database context for the Trading Engine.
/// Orchestrates the Entity Framework Core mapping between Domain Entities and the SQLite relational schema.
/// </summary>
public class TradingDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the Companies table, representing market agents.
    /// </summary>
    public DbSet<Company> Companies { get; set; }

    /// <summary>
    /// Gets or sets the Wallets table, managing financial liquidity.
    /// </summary>
    public DbSet<Wallet> Wallets { get; set; }

    /// <summary>
    /// Gets or sets the Inventories table, tracking physical asset stocks.
    /// </summary>
    public DbSet<Inventory> Inventories { get; set; }

    /// <summary>
    /// Gets or sets the TradeHistories table, storing immutable transaction logs.
    /// </summary>
    public DbSet<TradeHistory> TradeHistories { get; set; }

    /// <summary>
    /// Configures the database engine and connection string.
    /// In this project, SQLite is used for lightweight, local persistence.
    /// </summary>
    /// <param name="optionsBuilder">The builder used to configure the context.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=trading_engine.db");
    }

    /// <summary>
    /// Defines the advanced relational mapping (Fluent API) for the database schema.
    /// Sets up constraints, primary keys, and complex relationships.
    /// </summary>
    /// <param name="modelBuilder">The builder used to define the model structure.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- Wallet Configuration ---
        // Explicitly defines the CompanyId as the Primary Key for the Wallet table.
        modelBuilder.Entity<Wallet>().HasKey(w => w.CompanyId);

        // --- One-to-One Relationship (Company <-> Wallet) ---
        // Ensures that each Company has exactly one Wallet, sharing the same Primary Key.
        modelBuilder.Entity<Company>()
            .HasOne(c => c.Wallet)
            .WithOne(w => w.Company)
            .HasForeignKey<Wallet>(w => w.CompanyId);

        // --- One-to-Many Relationship (Company <-> Inventories) ---
        // Configures the relationship where a single Company owns multiple Inventory asset records.
        modelBuilder.Entity<Company>()
            .HasMany(c => c.Inventories)
            .WithOne(i => i.Company)
            .HasForeignKey(i => i.CompanyId);
    }
}