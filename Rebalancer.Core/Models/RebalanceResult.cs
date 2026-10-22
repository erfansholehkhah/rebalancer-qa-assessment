

using Rebalancer.Core.Calculation;
using System.Globalization;

namespace Rebalancer.Core.Models
{
    public class RebalanceResult
    {
        public string AccountId { get; }

        public IReadOnlyList<TradeInstruction> Trades { get; }

        public RebalanceResult(string accountId, IReadOnlyList<TradeInstruction> trades)
        {
            AccountId = accountId;
            Trades = trades;
        }

        public decimal TotalBuyValue => Trades.Where(t => t.Action == TradeAction.Buy).Sum(t => t.TradeValue);

        public decimal TotalSellValue => Trades.Where(t => t.Action == TradeAction.Sell).Sum(t => t.TradeValue);


        public TradeInstruction For(string stockName) => Trades.Single(t => t.StockName == stockName);


    }
}
