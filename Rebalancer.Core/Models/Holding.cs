

using System.Diagnostics.SymbolStore;

namespace Rebalancer.Core.Models
{
    public class Holding
    {
        public string Symbol { get; }
        public decimal TargetPercent { get; }
        public decimal CurrentPercent { get; }

        public decimal UnitPrice { get; }

        public decimal TargetVariance => CurrentPercent - TargetPercent;

        public Holding(string symbol,decimal targetPercent,decimal currentPercent,decimal unitPrice) 
        {
            Symbol = symbol;
            TargetPercent =  targetPercent;
            CurrentPercent = currentPercent;
            UnitPrice = unitPrice;

        }
        
    }
}
