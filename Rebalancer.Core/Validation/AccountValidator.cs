using System.Text.RegularExpressions;
using Rebalancer.Core.Exceptions;
using Rebalancer.Core.Models;

namespace Rebalancer.Core.Validation;

public class AccountValidator : IAccountValidator 
    // Fully asked AI  to create this file 
{
    
    private static readonly Regex StockNamePattern = new(@"^[A-Z]{1,5}(\.[A-Z])?$");

    
    private const decimal PercentTolerance = 0.01m;

   
    private static readonly List<HoldingRule> HoldingRules = new()
    {
        new HoldingRule(
            h => !string.IsNullOrWhiteSpace(h.StockName) && StockNamePattern.IsMatch(h.StockName),
            h => $"'{h.StockName}': stock name must be 1-5 capital letters, e.g. IBM or BRK.B."),

        new HoldingRule(
            h => h.UnitPrice > 0,
            h => $"{h.StockName}: unit price must be greater than 0 (was {h.UnitPrice})."),

        new HoldingRule(
            h => h.TargetPercent >= 0 && h.TargetPercent <= 100,
            h => $"{h.StockName}: target % must be between 0 and 100 (was {h.TargetPercent})."),

        new HoldingRule(
            h => h.CurrentPercent >= 0 && h.CurrentPercent <= 100,
            h => $"{h.StockName}: current % must be between 0 and 100 (was {h.CurrentPercent}).")
    };

    public void Validate(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);

        var errors = new List<string>();

        if (account.TotalAssets <= 0)
            errors.Add($"Total assets must be greater than 0 (was {account.TotalAssets}).");

        if (account.Holdings is null || account.Holdings.Count == 0)
        {
            errors.Add("The account must contain at least one holding.");
            throw new InvalidAccountException(errors);
        }

        foreach (var holding in account.Holdings)
        {
            foreach (var rule in HoldingRules)
            {
                if (!rule.IsValid(holding))
                    errors.Add(rule.ErrorMessage(holding));
            }
        }

        
        var duplicates = account.Holdings
            .GroupBy(h => h.StockName)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);

        foreach (var name in duplicates)
            errors.Add($"{name}: stock appears more than once.");

        
        var targetSum = account.Holdings.Sum(h => h.TargetPercent);
        if (Math.Abs(targetSum - 100) > PercentTolerance)
            errors.Add($"Target % must add up to 100 (was {targetSum}).");

        var currentSum = account.Holdings.Sum(h => h.CurrentPercent);
        if (Math.Abs(currentSum - 100) > PercentTolerance)
            errors.Add($"Current % must add up to 100 (was {currentSum}).");

        if (errors.Count > 0)
            throw new InvalidAccountException(errors);
    }
}