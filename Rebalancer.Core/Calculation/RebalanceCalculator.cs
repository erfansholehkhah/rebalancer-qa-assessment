

using Rebalancer.Core.Models;

namespace Rebalancer.Core.Calculation
{
    public class RebalanceCalculator : IRebalanceCalculator
    {
        public RebalanceResult Calculate(Account account)
        {
            var trades = account.Holdings
                .Select(Holding => CreateTrade(Holding, account.TotalAssets)).ToList();
            return new RebalanceResult(account.AccountId, trades);
        }

        public static TradeInstruction CreateTrade(Holding holding, decimal totalAssets)
        {
            var variance = holding.TargetVariance;

            if (variance == 0)
                return new TradeInstruction(holding.StockName, TradeAction.Keep, 0, holding.UnitPrice);

            var tradeAmount = Math.Abs(variance) / 100m * totalAssets;

            var shares = Math.Truncate(tradeAmount / holding.UnitPrice);

            if (shares == 0)
                return new TradeInstruction(holding.StockName, TradeAction.Keep, 0, holding.UnitPrice);

            var action = variance < 0 ? TradeAction.Buy : TradeAction.Sell; 

            return new TradeInstruction(holding.StockName, action, shares, holding.UnitPrice);
        }
    }
}
