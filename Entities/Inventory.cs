namespace MultiDomainTradingEngine.Entities;

using System;



/// <summary>
/// Represents a specific physical asset or commodity stock held by a Company.
/// </summary>
public class Inventory
{
    /// <summary>
    /// Unique identifier for the inventory record (PK).
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Reference to the owning Company (FK).
    /// </summary>
    public Guid CompanyId { get; private set; }

    /// <summary>
    /// The shorthand ticker or symbol (e.g., "WOOD", "STEEL").
    /// </summary>
    public string AssetSymbol { get; private set; } = string.Empty;

    /// <summary>
    /// The current amount available. Protected from external direct manipulation.
    /// </summary>
    public int Quantity { get; private set; }

    // --- Navigation Properties ---
    public Company? Company { get; private set; }

    /// <summary>
    /// Required by Entity Framework Core.
    /// </summary>
    protected Inventory() { }

    /// <summary>
    /// Main constructor for creating assets with business integrity.
    /// Resolves the "constructor takes 3 arguments" error.
    /// </summary>
    public Inventory(Guid companyId, string assetSymbol, int quantity)
    {
        if (companyId == Guid.Empty) throw new ArgumentException("Inventory must belong to a company.");
        if (string.IsNullOrWhiteSpace(assetSymbol)) throw new ArgumentException("Asset symbol is required.");
        if (quantity < 0) throw new ArgumentException("Initial quantity cannot be negative.");

        Id = Guid.NewGuid();
        CompanyId = companyId;
        AssetSymbol = assetSymbol;
        Quantity = quantity;
    }

    // --- Business Behaviors ---

    /// <summary>
    /// Increases stock after a purchase or production.
    /// </summary>
    public void AddStock(int amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        Quantity += amount;
    }

    /// <summary>
    /// Decreases stock after a sale, preventing negative inventory.
    /// </summary>
    public bool RemoveStock(int amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        if (Quantity < amount) return false; // Estoque insuficiente

        Quantity -= amount;
        return true;
    }
}