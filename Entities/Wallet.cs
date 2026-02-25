namespace MultiDomainTradingEngine.Entities;

using System;

/// <summary>
/// Represents the central financial ledger for a <see cref="Corporation"/>.
/// The Wallet is responsible for managing fiat liquidity, enforcing balance integrity, 
/// and providing atomic operations for capital flow.
/// </summary>
public class Wallet
{   
    /// <summary>
    /// Gets or sets the unique identifier of the owning corporation.
    /// In this 1:1 relationship pattern, this property serves as both the 
    /// Primary Key and the Foreign Key.
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Gets the current available liquidity. 
    /// Direct set is restricted to maintain transactional integrity; 
    /// modifications should occur via <see cref="Deposit"/> or <see cref="Withdraw"/>.
    /// </summary>
    public decimal Balance { get; private set; }

    // --- Navigation Properties ---

    /// <summary>
    /// Navigation property to the parent <see cref="Corporation"/>.
    /// Linked via the GUID-based identity pattern.
    /// </summary>
    public virtual Corporation? Company { get; private set; }

    /// <summary>
    /// Required parameterless constructor for Entity Framework Core materialization.
    /// </summary>
    public Wallet() { }

    /// <summary>
    /// Initializes a new financial wallet with baseline business constraints.
    /// </summary>
    /// <param name="companyId">The unique identifier of the associated market agent.</param>
    /// <param name="initialBalance">The starting capital to be injected into the agent's account.</param>
    /// <exception cref="ArgumentException">Thrown if the company reference is null or the balance is negative.</exception>
    public Wallet(Guid companyId, decimal initialBalance)
    {
        if (companyId == Guid.Empty) 
            throw new ArgumentException("A Wallet cannot exist without a valid Company link.");

        if (initialBalance < 0)
            throw new ArgumentException("Initial liquidity injection cannot be negative.");

        CompanyId = companyId;
        Balance = initialBalance;
    }

    // --- Domain Behaviors (Transactional Logic) ---

    /// <summary>
    /// Safely credit funds to the wallet balance.
    /// </summary>
    /// <param name="amount">The positive decimal value to be added.</param>
    /// <exception cref="ArgumentException">Thrown if the deposit amount is zero or negative.</exception>
    public void Deposit(decimal amount)
    {
        if (amount <= 0) 
            throw new ArgumentException("Credit operations require a positive amount.");
        
        Balance += amount;
    }

    /// <summary>
    /// Safely attempts to debit funds from the wallet, preventing an overdraft state.
    /// </summary>
    /// <param name="amount">The positive decimal value to be removed.</param>
    /// <returns>True if the transaction was successful; False if liquidity is insufficient.</returns>
    /// <exception cref="ArgumentException">Thrown if the withdrawal amount is zero or negative.</exception>
    public bool Withdraw(decimal amount)
    {
        if (amount <= 0) 
            throw new ArgumentException("Debit operations require a positive amount.");
        
        if (Balance < amount) 
            return false; // Insufficient liquidity for transaction

        Balance -= amount;
        return true;
    }
}