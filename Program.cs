/// <summary>
/// The entry point of the Multi-Domain Trading Engine (MDTE).
/// This script orchestrates the bootstrap process, encompassing database schema reset,
/// environment seeding, and the automated assembly of market agents.
/// </summary>

/* * ARCHITECTURAL FLOW:
 * Step 1: Infrastructure Reset -> Ensures a clean state for GUID-based relations.
 * Step 2: Economic Seeding -> Populates the "Economic DNA" (Sectors) from external JSON.
 * Step 3: Agent Assembly -> Executes the factory pipeline for Corporations and Wallets.
 * Step 4: Market Validation -> Uses Eager Loading to verify relational integrity.
 */

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MultiDomainTradingEngine.Data;
using MultiDomainTradingEngine.Entities;
using MultiDomainTradingEngine.Factory.Sectors;
using MultiDomainTradingEngine.Factory.Corporation;
using MultiDomainTradingEngine.Factory.Wallet;

// ----------------------------------------------------------------------------------
// DATA CONTEXT INITIALIZATION
// ----------------------------------------------------------------------------------
using (var db = new TradingDbContext())
{
    // STEP 1: INFRASTRUCTURE REBOOT
    // Wipes the existing SQLite file to prevent schema mismatch after GUID migration.
    db.Database.EnsureDeleted(); 
    db.Database.EnsureCreated();

    // STEP 2: SECTOR BOOTSTRAPPING
    // Injects the industry verticals required for corporation identity mapping.
    var sectorGenerator = new GeneratorSector(db);
    await sectorGenerator.InitializeSectorsAsync("Factory/Sectors/Sectors.json");

    // STEP 3: CONTEXTUAL ASSEMBLY LINE (Pipeline)
    // Instantiates factories to build complex entities with linked dependencies.
    var corpGenerator = new GeneratorCorporation(db);
    var walletGenerator = new GeneratorWallet(db);
    
    for (int i = 0; i < 10; i++)
    {
        // Phase A: Identity Construction (Assigning a random Sector GUID)
        var corp = await corpGenerator.BuildIdentityAsync($"Global Corp {i}", "S/A");
        
        // Phase B: Financial Onboarding (Linked via 1:1 relationship)
        var wallet = await walletGenerator.BuildWalletAsync(corp.Id, 1000000m); 
        
        // Phase C: Rule Application (Applying business logic via Extension Method)
        wallet.Deposit(50000m); 
    }

    // STEP 4: RELATIONAL VALIDATION (Eager Loading)
    // Uses .Include() to perform SQL JOINs, bringing related data into memory.
    var agents = db.Corporations
        .Include(c => c.CoreSector) // Fetches the linked Industry info
        .Include(c => c.Wallet)     // Fetches the protected balance info
        .ToList();

    foreach (var a in agents)
    {
        // Logging the results for audit and verification
        string sectorName = a.CoreSector?.Name ?? "N/A";
        string cash = a.Wallet?.Balance.ToString("N2") ?? "0.00";
        Console.WriteLine($"CORP: {a.Name} | SECTOR: {sectorName} | BALANCE: ${cash}");
    }
}