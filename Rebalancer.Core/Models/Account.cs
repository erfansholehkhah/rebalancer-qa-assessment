

namespace Rebalancer.Core.Models
{
    public class Account
    {
        public string AccountId { get; }
        public decimal TotalAssets { get; }
        public IReadOnlyList<Holding> Holdings { get; }


        public Account (string accountId, decimal totalAssets , IReadOnlyList<Holding> holdings) 
        {
            AccountId = accountId;
            TotalAssets = totalAssets;
            Holdings = holdings;
        }
    }
}
