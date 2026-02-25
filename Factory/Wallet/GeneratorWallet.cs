namespace MultiDomainTradingEngine.Factory.Wallet;

using MultiDomainTradingEngine.Entities;
using MultiDomainTradingEngine.Data;
using System;
using System.Threading.Tasks;

/// <summary>
/// Facilitates the financial onboarding of corporations by initializing their dedicated wallets.
/// This factory handles the creation of the 1:1 financial relationship between a market agent 
/// and its liquid capital.
/// </summary>
public class GeneratorWallet
{
    private readonly TradingDbContext _context;

    /// <summary>
    /// Initializes the wallet factory with a scoped database context.
    /// </summary>
    /// <param name="context">The primary data gateway for financial record persistence.</param>
    public GeneratorWallet(TradingDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Asynchronously assembles and persists a new <see cref="Wallet"/> for a specific corporation.
    /// This represents the "Capital Injection" phase of the corporate assembly pipeline.
    /// </summary>
    /// <param name="corporationId">The unique GUID of the corporation that will own the wallet.</param>
    /// <param name="initialBalance">The starting fiat liquidity to be allocated to the account.</param>
    /// <returns>A persisted <see cref="Wallet"/> instance linked to the corporation.</returns>
    /// <remarks>
    /// This method enforces the 1:1 relational constraint where the CompanyId acts as 
    /// both the Primary Key and Foreign Key.
    /// </remarks>
    public async Task<Wallet> BuildWalletAsync(Guid corporationId, decimal initialBalance)
    {
        // Initialization: Constructing the financial ledger with the provided identity and capital.
        var wallet = new Wallet(corporationId, initialBalance);

        // Persistence: Adding the wallet to the change tracker and committing to the database.
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();
        
        return wallet;
    }
}