namespace MultiDomainTradingEngine.Factory.Corporation;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultiDomainTradingEngine.Data;
using MultiDomainTradingEngine.Entities;

/// <summary>
/// Orchestrates the instantiation and persistence of corporate entities within the market.
/// Acts as a specialized factory that enforces relational constraints, such as ensuring 
/// every new agent is assigned a valid economic sector.
/// </summary>
public class GeneratorCorporation
{
    private readonly TradingDbContext _context;

    /// <summary>
    /// Initializes a new instance of the factory with a scoped database context.
    /// </summary>
    /// <param name="context">The primary data gateway for entity persistence.</param>
    public GeneratorCorporation(TradingDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Executes the "Identity Construction" phase of the pipeline. 
    /// This method performs an atomic operation to create a new corporation, link it to a 
    /// randomized sector, and persist the record to the database.
    /// </summary>
    /// <param name="name">The assigned commercial name of the entity.</param>
    /// <param name="suffix">The legal suffix representing the company's structure.</param>
    /// <returns>A fully initialized and tracked <see cref="Corporation"/> instance.</returns>
    /// <exception cref="Exception">Thrown if the 'Sectors' table is empty, preventing relational mapping.</exception>
    /// <remarks>
    /// Note: Due to SQLite limitations with GUID randomization, <see cref="Enumerable.AsEnumerable"/> 
    /// is used to perform client-side evaluation for sector selection.
    /// </remarks>
    public async Task<Corporation> BuildIdentityAsync(string name, string suffix)
    {
        // Selection Logic: Fetch sectors and randomize on the client side to bypass SQLite limitations.
        var randomSector = _context.Sectors
            .AsEnumerable() 
            .OrderBy(s => Guid.NewGuid())
            .FirstOrDefault();
        
        // Relational Guard: Ensure the "Economic DNA" exists before allowing an agent to be born.
        if (randomSector == null) 
            throw new Exception("Assembly Line Stalled: Sectors must be seeded before creating Corporations.");

        // Initialization: Constructing the entity with a fresh GUID and linking the Foreign Key.
        var corporation = new Corporation(name, suffix, randomSector.Id);
        
        _context.Corporations.Add(corporation);
        
        // Persistence: Commit the new agent to the database.
        await _context.SaveChangesAsync();

        return corporation; 
    }
}