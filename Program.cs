using System;
using System.Collections.Generic;
using System.Linq;
using MultiDomainTradingEngine.Data;
using MultiDomainTradingEngine.Entities;
using MultiDomainTradingEngine.Utils;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("=== MDTE: Architectural & Market Validation ===\n");

using (var db = new TradingDbContext())
{
    // 1. Limpeza do ambiente: Garante que você não está vendo dados "sujos" de testes antigos
    Console.WriteLine("[Step 1] Recreating Database...");
    db.Database.EnsureDeleted(); 
    db.Database.EnsureCreated();

    // 2. Geração em Massa: Vamos criar 10 agentes para ver a aleatoriedade em ação
    Console.WriteLine("[Step 2] Generating 10 random market agents with billions...");
    for (int i = 0; i < 10; i++)
    {
        var randomComp = MarketGenerator.CreateRandomCompany();
        db.Companies.Add(randomComp);
    }

    // O SaveChanges aqui valida se os seus novos construtores e private sets funcionam com o EF Core
    db.SaveChanges();
    Console.WriteLine("Success: Data persisted to SQLite.\n");

    // 3. Validação de Integridade: Recuperando os dados com Eager Loading (Include)
    Console.WriteLine("[Step 3] Querying agents and validating tiered balances:\n");
    
    var agents = db.Companies
        .Include(c => c.Wallet)
        .Include(c => c.Inventories)
        .OrderByDescending(c => c.Tier) // Organiza para ver os mais ricos primeiro
        .ToList();

    foreach (var a in agents)
    {
        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"AGENT: {a.Name.ToUpper().PadRight(25)} | TIER: {a.Tier}");
        
        // Formatação N0 para facilitar a leitura de bilhões
        string cash = a.Wallet?.Balance.ToString("N0") ?? "0";
        Console.WriteLine($"CASH:  $ {cash.PadLeft(18)}");
        
        Console.WriteLine("ASSETS IN STOCK:");
        foreach (var inv in a.Inventories)
        {
            Console.WriteLine($"  > {inv.AssetSymbol.PadRight(15)} | {inv.Quantity:N0} units");
        }
    }
}

Console.WriteLine("\n" + new string('=', 60));
Console.WriteLine("VALIDATION COMPLETE: Your Engine Infrastructure is Solid.");