namespace Rebalancer.Core.Models
{
    public class Holding
    {
        public string StockName { get; }
        public decimal TargetPercent { get; }
        public decimal CurrentPercent { get; }

        public decimal UnitPrice { get; }

        public decimal TargetVariance => CurrentPercent - TargetPercent;

        public Holding(string stockName, decimal targetPercent, decimal currentPercent, decimal unitPrice)
        {
            StockName = stockName;
            TargetPercent = targetPercent;
            CurrentPercent = currentPercent;
            UnitPrice = unitPrice;

        }

    }
}
