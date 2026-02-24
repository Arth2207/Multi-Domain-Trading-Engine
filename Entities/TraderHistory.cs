namespace MultiDomainTradingEngine.Entities;

using System;





/// <summary>
/// Represents an immutable record of a completed transaction between two market agents.
/// This entity serves as the primary source of truth for audit trails and financial reporting.
/// </summary>
public class TradeHistory
{
    /// <summary>
    /// Unique identifier for the trade execution.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Reference to the Company that purchased the asset.
    /// </summary>
    public Guid BuyerId { get; set; }

    /// <summary>
    /// Reference to the Company that sold the asset.
    /// </summary>
    public Guid SellerId { get; set; }

    /// <summary>
    /// The ticker symbol of the asset exchanged (e.g., "GOLD", "IRON").
    /// </summary>
    public string AssetSymbol { get; set; } = string.Empty;

    /// <summary>
    /// The agreed-upon price per unit at the moment of execution.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Total number of units successfully transferred.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The exact UTC timestamp when the matching engine finalized the trade.
    /// This property is critical for time-series analysis and historical tracking.
    /// </summary>
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}