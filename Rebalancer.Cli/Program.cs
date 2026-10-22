using Rebalancer.Core.Calculation;
using Rebalancer.Core.Models;
namespace Rebalancer.Cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var account = new Account("ABC", 100_000m, new List<Holding>
            {
                new Holding ("IBM",20,10,150),
                new Holding ("MSTF",20,20,90),
                new Holding ("ORCL",20,30,220),
                new Holding ("AAPL",20,20,450),
                new Holding ("HD",20,20,70),
                
            });
            IRebalanceCalculator calculator = new RebalanceCalculator();
            var result = calculator.Calculate(account);

            foreach (var trade in result.Trades) 
            {
                Console.WriteLine($"{trade.StockName,-5} {trade.Action,-5} {trade.Shares,3} shares  ${trade.TradeValue}");
            }

            Console.WriteLine();
            Console.WriteLine($"Total sells: ${result.TotalSellValue}");
            Console.WriteLine($"Total buys:  ${result.TotalBuyValue}");
        }
    }
}
