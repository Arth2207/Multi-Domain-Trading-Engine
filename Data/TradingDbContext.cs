namespace MultiDomainTradingEngine.Data;

using Microsoft.EntityFrameworkCore;
using MultiDomainTradingEngine.Entities;

public class TradingDbContext : DbContext
{
    public DbSet<Corporation> Corporations { get; set; }
    public DbSet<Sector> Sectors { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Inventory> Inventories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=trading_engine.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- 1. Relação 1:N (Sector <-> Corporation) ---
        // O Core de cada empresa vindo da sua Factory de Setores
        modelBuilder.Entity<Corporation>()
            .HasOne(c => c.CoreSector)
            .WithMany(s => s.CorporationsWithThisCore)
            .HasForeignKey(c => c.CoreSectorId);

        // --- 2. Relação 1:1 (Corporation <-> Wallet) ---
        // Link direto para saber a qual empresa a carteira pertence
        modelBuilder.Entity<Wallet>().HasKey(w => w.CompanyId);

        modelBuilder.Entity<Corporation>()
            .HasOne(c => c.Wallet)
            .WithOne(w => w.Company)
            .HasForeignKey<Wallet>(w => w.CompanyId);

        // --- 3. Relação 1:N (Corporation <-> Inventory) ---
        // O link para o inventário que você quer tratar como "documento"
        modelBuilder.Entity<Inventory>()
            .HasOne(i => i.Company)
            .WithMany(c => c.Inventories)
            .HasForeignKey(i => i.CompanyId);
    }
}