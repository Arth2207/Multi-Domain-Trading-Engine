namespace MultiDomainTradingEngine.Entities;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;



/// <summary>
/// Represents a Market Agent (Corporation) within the trading ecosystem.
/// Acts as the Aggregate Root for managing associated financial liquidity and asset inventories.
/// </summary>
public class Company
{
    /// <summary>
    /// Gets the unique identifier for the Company.
    /// Serves as the Primary Key (PK) for database persistence.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the registered trade name of the corporation.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the classification tier of the company (e.g., "S", "A", "B", "C").
    /// Used by the engine to gate access to specific high-tier trading domains and privileges.
    /// </summary>
    public string Tier { get; private set; }

    // --- Navigation Properties (Entity Framework Core) ---

    /// <summary>
    /// The financial account associated with this entity.
    /// Established as a mandatory 1:1 relationship where the Wallet's PK is the Company's FK.
    /// </summary>
    public Wallet? Wallet { get; private set; }

    /// <summary>
    /// Backing field for the inventories collection to prevent external direct manipulation.
    /// </summary>
    private readonly List<Inventory> _inventories = new();

    /// <summary>
    /// Gets a read-only collection of physical assets or commodities owned by the company.
    /// Represents a 1:N relationship managed through domain-driven behaviors.
    /// </summary>
    public IReadOnlyCollection<Inventory> Inventories => _inventories.AsReadOnly();

    /// <summary>
    /// Parameterless constructor required by Entity Framework Core for data materialization.
    /// </summary>
    protected Company() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Company"/> class with required business data.
    /// </summary>
    /// <param name="name">The corporation's official name.</param>
    /// <param name="tier">The market standing tier (S, A, B, or C).</param>
    /// <exception cref="ArgumentException">Thrown when name or tier is null or whitespace.</exception>
    public Company(string name, string tier)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name cannot be empty.", nameof(name));
            
        if (string.IsNullOrWhiteSpace(tier))
            throw new ArgumentException("Market tier must be specified.", nameof(tier));

        Id = Guid.NewGuid();
        Name = name;
        Tier = tier;
    }

    /// <summary>
    /// Assigns a financial wallet to the company, ensuring business rules are enforced.
    /// </summary>
    /// <param name="wallet">The wallet instance to link.</param>
    public void AssignWallet(Wallet wallet)
    {
        ArgumentNullException.ThrowIfNull(wallet);
        Wallet = wallet;
    }

    /// <summary>
    /// Safely adds a physical asset to the company's inventory through a controlled domain method.
    /// </summary>
    /// <param name="inventory">The inventory record to add.</param>
    public void ReceiveAsset(Inventory inventory)
    {
        ArgumentNullException.ThrowIfNull(inventory);
        
        // Prevents registering the same inventory object twice to maintain data integrity.
        if (!_inventories.Contains(inventory))
        {
            _inventories.Add(inventory);
        }
    }
}