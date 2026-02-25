using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultiDomainTradingEngine.Data;
using MultiDomainTradingEngine.Entities;
using MultiDomainTradingEngine.Factory.Sectors;     // Verifique se a pasta é Sectors ou Sector
using MultiDomainTradingEngine.Factory.Corporation; 
using MultiDomainTradingEngine.Factory.Wallet;
using MultiDomainTradingEngine.Rules.Wallet;

Console.WriteLine("=== MDTE: Architectural & Market Validation (Senior Version) ===\n");

using (var db = new TradingDbContext())
{
    // 1. Reset Total do Ambiente (Deleta o banco antigo e cria o novo com GUIDs)
    Console.WriteLine("[Step 1] Recreating Database (Cleaning old schema)...");
    db.Database.EnsureDeleted(); 
    db.Database.EnsureCreated();

    // 2. Estação 1: Infraestrutura de Setores
    Console.WriteLine("[Step 2] Seeding Sectors from JSON...");
    var sectorGenerator = new GeneratorSector(db);
    await sectorGenerator.InitializeSectorsAsync("Factory/Sectors/Sectors.json");

    // 3. Estação 2 & 3: Linha de Montagem de Agentes
    var corpGenerator = new GeneratorCorporation(db);
    var walletGenerator = new GeneratorWallet(db);
    
    Console.WriteLine("[Step 3] Assembling 10 Corporations via Pipeline...");
    for (int i = 0; i < 10; i++)
    {
        // Cria a Identidade (Sorteando Setor Guid do banco)
        var corp = await corpGenerator.BuildIdentityAsync($"Global Corp {i}", "S/A");
        
        // Cria a Wallet vinculada
        var wallet = await walletGenerator.BuildWalletAsync(corp.Id, 1000000m); 
        
        // Aplica regra de negócio via Extension Method
        wallet.Deposit(50000m); 
    }

    Console.WriteLine("Success: Pipeline execution complete.\n");

    // 4. Validação (Agora trazendo o CORE SECTOR no resultado)
    Console.WriteLine("[Step 4] Querying agents with Eager Loading:\n");
    
    var agents = db.Corporations
        .Include(c => c.CoreSector) // JOIN fundamental para trazer o nome do setor
        .Include(c => c.Wallet)     
        .ToList();

    foreach (var a in agents)
    {
        Console.WriteLine(new string('-', 70));
        Console.WriteLine($"CORP: {a.Name} {a.Suffixes}");
        
        // Acessando os dados do Setor que agora estão na memória
        string sectorName = a.CoreSector?.Name ?? "N/A";
        string primary = a.CoreSector?.PrimarySector ?? "N/A";
        
        Console.WriteLine($"CORE SECTOR: {sectorName} ({primary})");
        
        string cash = a.Wallet?.Balance.ToString("N2") ?? "0.00";
        Console.WriteLine($"WALLET BALANCE: $ {cash.PadLeft(20)}");
    }
}

Console.WriteLine("\n" + new string('=', 70));
Console.WriteLine("VALIDATION COMPLETE: Database reset and Core Sector is visible.");