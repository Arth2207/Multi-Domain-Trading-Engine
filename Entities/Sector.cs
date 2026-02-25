namespace MultiDomainTradingEngine.Entities;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a distinct economic vertical within the global market.
/// Sectors act as the "Economic DNA" for corporations, influencing their 
/// production capabilities, valuation, and market behavior.
/// </summary>
public class Sector
{
    /// <summary>
    /// Gets the unique identifier for the sector.
    /// Primary Key used for GUID-based relational mapping.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets or sets the specific industry name (e.g., "Quantum Computing", "Deep Sea Mining").
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the broader economic category (e.g., "Technology", "Resources").
    /// Used for grouping and macro-economic analysis.
    /// </summary>
    public string PrimarySector { get; set; } = null!;

    /// <summary>
    /// Gets or sets the multiplier or grade used to calculate asset values 
    /// and corporate health within this specific industry.
    /// </summary>
    public double ValuationGrade { get; set; }
    
    /// <summary>
    /// Navigation property containing all corporations that use this sector as their primary identity.
    /// Enables reverse-lookup from an economic vertical to its market agents.
    /// </summary>
    public virtual ICollection<Corporation> CorporationsWithThisCore { get; set; } = new List<Corporation>();

    /// <summary>
    /// Required constructor for Entity Framework Core and manual initialization.
    /// </summary>
    public Sector()
    {
        Id = Guid.NewGuid();
    }
}