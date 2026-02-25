namespace MultiDomainTradingEngine.Entities;

using System;

/// <summary>
/// Represents a discrete stock of a physical asset or commodity held by a market agent.
/// This entity tracks resource availability and enforces transactional integrity 
/// during trade execution or resource consumption.
/// </summary>
public class Inventory
{
    /// <summary>
    /// Gets the unique identifier for this inventory record.
    /// Primary Key used for database persistence and entity tracking.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the Foreign Key reference to the <see cref="Corporation"/> that owns this stock.
    /// </summary>
    public Guid CompanyId { get; private set; }

    /// <summary>
    /// Gets the shorthand ticker or unique identifier for the asset (e.g., "GOLD", "OIL").
    /// </summary>
    public string AssetSymbol { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the current units available. 
    /// Direct manipulation is prohibited to ensure state consistency; 
    /// use <see cref="AddStock"/> or <see cref="RemoveStock"/> for adjustments.
    /// </summary>
    public int Quantity { get; private set; }

    // --- Navigation Properties ---

    /// <summary>
    /// Navigation property to the owning corporation. 
    /// Initialized as null and populated via Eager Loading in the data context.
    /// </summary>
    public virtual Corporation? Company { get; private set; }

    /// <summary>
    /// Required protected constructor for Entity Framework Core materialization.
    /// Ensures objects can be reconstituted from the database without invoking business logic.
    /// </summary>
    protected Inventory() { }

    /// <summary>
    /// Initializes a new instance of an asset stock with enforced business rules.
    /// </summary>
    /// <param name="companyId">The unique identifier of the owning corporation.</param>
    /// <param name="assetSymbol">The symbol representing the commodity.</param>
    /// <param name="quantity">The initial volume of stock to be allocated.</param>
    /// <exception cref="ArgumentException">Thrown if inputs violate domain integrity rules.</exception>
    public Inventory(Guid companyId, string assetSymbol, int quantity)
    {
        if (companyId == Guid.Empty) 
            throw new ArgumentException("Inventory must belong to a valid company.");
        if (string.IsNullOrWhiteSpace(assetSymbol)) 
            throw new ArgumentException("Asset symbol is required.");
        if (quantity < 0) 
            throw new ArgumentException("Initial quantity cannot be negative.");

        Id = Guid.NewGuid();
        CompanyId = companyId;
        AssetSymbol = assetSymbol;
        Quantity = quantity;
    }

    // --- Domain Behaviors (Business Logic) ---

    /// <summary>
    /// Safely increases the stock quantity following a successful purchase or production cycle.
    /// </summary>
    /// <param name="amount">The positive integer to add to the current balance.</param>
    public void AddStock(int amount)
    {
        if (amount <= 0) 
            throw new ArgumentException("Increment amount must be positive.");

        Quantity += amount;
    }

    /// <summary>
    /// Safely decreases the stock quantity, preventing the entity from entering a negative state.
    /// </summary>
    /// <param name="amount">The positive integer to subtract from the current balance.</param>
    /// <returns>True if the operation succeeded; False if there is insufficient stock.</returns>
    public bool RemoveStock(int amount)
    {
        if (amount <= 0) 
            throw new ArgumentException("Deduction amount must be positive.");
        
        if (Quantity < amount)
            return false; // Insufficient inventory state

        Quantity -= amount;
        return true;
    }
}