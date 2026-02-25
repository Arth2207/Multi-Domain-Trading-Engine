namespace MultiDomainTradingEngine.Entities;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents the core business entity within the trading engine.
/// A Corporation serves as the primary market agent, possessing financial resources, 
/// specialized sector alignment, and a physical inventory of assets.
/// </summary>
public class Corporation
{
    /// <summary>
    /// Gets the unique identifier for this corporation. 
    /// Used as the primary key for database persistence.
    /// </summary>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Gets the display name of the corporation.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Gets the legal suffix (e.g., "S/A", "Holdings", "Partners").
    /// </summary>
    public string Suffixes { get; private set; } = null!;

    /// <summary>
    /// Gets the UTC timestamp when this entity was first initialized in the market.
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// Gets or sets the Foreign Key for the primary economic sector this corporation operates in.
    /// </summary>
    public Guid CoreSectorId { get; set; }

    /// <summary>
    /// Navigation property for the primary economic sector.
    /// </summary>
    public virtual Sector CoreSector { get; set; } = null!;

    /// <summary>
    /// Navigation property for secondary or related sectors this corporation interacts with.
    /// </summary>
    public virtual ICollection<Sector> AdjacentSectors { get; set; } = new List<Sector>();

    /// <summary>
    /// Navigation property for the corporation's financial engine (Cash management).
    /// </summary>
    public virtual Wallet Wallet { get; set; } = null!;

    /// <summary>
    /// Navigation property for the collection of assets currently held by the corporation.
    /// </summary>
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    /// <summary>
    /// Required protected constructor for Entity Framework Core materialization.
    /// </summary>
    protected Corporation() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Corporation"/> class with a fresh identity.
    /// </summary>
    /// <param name="name">The name of the company.</param>
    /// <param name="suffixes">The legal suffix to append.</param>
    /// <param name="coreSectorId">The Guid of the pre-existing Sector to link this agent to.</param>
    public Corporation(string name, string suffixes, Guid coreSectorId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Suffixes = suffixes;
        CoreSectorId = coreSectorId;
        CreatedAt = DateTime.UtcNow;
    }
}