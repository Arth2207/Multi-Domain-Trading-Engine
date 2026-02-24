namespace MultiDomainTradingEngine.Entities;

using System;


/// <summary>
/// Represents the financial account linked to a specific Company.
/// Responsible for maintaining fiat liquidity and ensuring transactional integrity.
/// </summary>
public class Wallet
{   
    /// <summary>
    /// Following the 1:1 relationship pattern, this acts as PK and FK.
    /// </summary>
    public Guid CompanyId { get; private set; }

    /// <summary>
    /// Current available balance. Protected from external direct manipulation.
    /// </summary>
    public decimal Balance { get; private set; }

    // --- Navigation Properties ---
    public Company? Company { get; private set; }

    /// <summary>
    /// Required by EF Core.
    /// </summary>
    protected Wallet() { }

    /// <summary>
    /// Initializes a new wallet with strict business rules.
    /// </summary>
    public Wallet(Guid companyId, decimal initialBalance)
    {
        if (companyId == Guid.Empty) 
            throw new ArgumentException("Wallet must be linked to a valid Company.");

        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative.");

        CompanyId = companyId;
        Balance = initialBalance;
    }

    // --- Business Behaviors (O que um Sênior faz) ---

    /// <summary>
    /// Safely adds funds to the wallet.
    /// </summary>
    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Deposit amount must be positive.");
        
        Balance += amount;
    }

    /// <summary>
    /// Safely removes funds, preventing negative balances.
    /// </summary>
    public bool Withdraw(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Withdrawal amount must be positive.");
        
        if (Balance < amount) return false; // Saldo insuficiente

        Balance -= amount;
        return true;
    }
}