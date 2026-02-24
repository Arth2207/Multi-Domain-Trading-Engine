namespace MultiDomainTradingEngine.Utils;

using System;
using MultiDomainTradingEngine.Entities;



public static class MarketGenerator {

    public static readonly Random _random = new();

    private static readonly string[] _companyNames = { "Apex", "Quantum", "Global", "Vanguard", "Horizon", "Titan" };
    private static readonly string[] _suffixes = { "Holdings", "Partners", "Capital", "Networks", "Systems" };
    private static readonly string[] _tiers = { "S", "A", "B", "C" };
    private static readonly string[] _assets = { "CRUDE_OIL", "GOLD_BULLION", "RARE_EARTH", "GRAIN", "SILICON" };


    public static Company CreateRandomCompany()
    {
        var name = $"{_companyNames[_random.Next(_companyNames.Length)]} {_suffixes[_random.Next(_suffixes.Length)]}";
        var tier = _tiers[_random.Next(_tiers.Length)];

        var company = new Company(name, tier);

        decimal minBalance, maxBalance;

        switch (tier)
        {
           case "S": 
            minBalance = 800_000_000m;
            maxBalance = 1_500_000_000m;
            break;
        case "A":
            minBalance = 200_000_000m;
            maxBalance = 800_000_000m;
            break;
        case "B":
            minBalance = 50_000_000m;
            maxBalance = 200_000_000m;
            break;
        default: // Tier C
            minBalance = 10_000_000m;
            maxBalance = 50_000_000m;
            break; 
        }

        var range  = (double)(maxBalance - minBalance);
        var randomValue = (decimal)(_random.NextDouble() * range) + minBalance;

        company.AssignWallet(new Wallet(company.Id, randomValue));

        int assetCount = _random.Next(2, 5);
        for(int i = 0; i < assetCount; i++)
        {
            var symbol = _assets[_random.Next(_assets.Length)];
            var quantity = _random.Next(100_000, 5_000_000);
            company.ReceiveAsset(new Inventory(company.Id, symbol, quantity));
        }

        
        return company;
    }



    
}