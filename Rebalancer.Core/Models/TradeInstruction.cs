

namespace Rebalancer.Core.Models
{
    public class TradeInstruction
    {
        public string StockName { get; }
        public TradeAction Action { get; }
        public decimal Shares { get; }

        public decimal UnitPrice { get; }


        public TradeInstruction(string stockName, TradeAction  action , decimal shares, decimal unitPrice )
        {

            StockName = stockName;
            Action = action;
            Shares = shares;
            UnitPrice = unitPrice;
            
        }

        public decimal TradeValue => Shares * UnitPrice;
    }

    
}
