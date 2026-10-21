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
            Console.WriteLine($"Account{account.AccountId}, total asset {account.TotalAssets}");

            foreach(var holding in account.Holdings) 
            {
                Console.WriteLine($"{holding.Symbol}:  Variance {holding.TargetVariance}");
            }
        }
    }
}
