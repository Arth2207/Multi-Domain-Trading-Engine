namespace MultiDomainTradingEngine.Factory.Sectors;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MultiDomainTradingEngine.Data;
using MultiDomainTradingEngine.Entities;

/// <summary>
/// Responsible for bootstrapping the market environment by seeding economic sectors from external data.
/// This factory ensures the system has a consistent foundation of industry verticals and baseline valuations.
/// </summary>
public class GeneratorSector
{
    private readonly TradingDbContext _context;

    /// <summary>
    /// Initializes a new instance of the sector factory.
    /// </summary>
    /// <param name="context">The database context for sector persistence.</param>
    public GeneratorSector(TradingDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Parses a source JSON file to populate the database with primary and specific economic sectors.
    /// Implements an idempotency check to prevent duplicate entries during environment resets.
    /// </summary>
    /// <param name="jsonPath">The physical file path to the sector seed data (e.g., sectors_seed.json).</param>
    /// <returns>A task representing the asynchronous seeding operation.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the source JSON configuration is missing.</exception>
    public async Task InitializeSectorsAsync(string jsonPath)
    {
        // Validation: Ensure the data source exists before attempting I/O operations.
        if (!File.Exists(jsonPath))
        {
            throw new FileNotFoundException("Sector seed configuration file not found.", jsonPath);
        }

        // Deserialization: Transforming raw JSON content into strongly-typed Data Transfer Objects (DTOs).
        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        var sectorSeeds = JsonSerializer.Deserialize<List<SectorSeedDto>>(jsonContent);

        if (sectorSeeds == null || sectorSeeds.Count == 0) return;

        foreach (var seed in sectorSeeds)
        {
            // Idempotency Logic: Verify existence by name to ensure data integrity across multiple runs.
            if (!_context.Sectors.Any(s => s.Name == seed.Name))
            {
                var newSector = new Sector
                {
                    Name = seed.Name,
                    PrimarySector = seed.PrimarySector,
                    // The ValuationGrade serves as the initial coefficient for the Python-based market model.
                    ValuationGrade = seed.BaseValuation 
                };

                _context.Sectors.Add(newSector);
            }
        }

        // Persistence: Commit all new industry verticals to the primary data store.
        await _context.SaveChangesAsync();
    }
}

/// <summary>
/// Immutable Data Transfer Object (DTO) used to map the external JSON structure to the internal domain model.
/// </summary>
/// <param name="PrimarySector">The high-level category (e.g., Technology, Healthcare).</param>
/// <param name="Name">The specific industry vertical name.</param>
/// <param name="BaseValuation">The starting valuation grade for market calculations.</param>
public record SectorSeedDto(string PrimarySector, string Name, double BaseValuation);