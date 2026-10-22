using Rebalancer.Core.Calculation;
using Rebalancer.Core.Models;
using Rebalancer.Core.Validation;

namespace Rebalancer.Tests
{

    [TestFixture]
      public class RebalanceCalculatorTests
    {
        private readonly IRebalanceCalculator _calculator = new RebalanceCalculator(new AccountValidator());

        private static Account CreateAccountAbc () 
        {
            return new Account("ABC", 100_000m, new List<Holding>
            {
             new Holding("IBM", 20, 10, 150),
            new Holding("MSFT", 20, 20, 90),
            new Holding("ORCL", 20, 30, 220),
            new Holding("AAPL", 20, 20, 450),
            new Holding("HD", 20, 20, 70)
            });
        }
        [TestCase("IBM", TradeAction.Buy, 66)]
        [TestCase("ORCL", TradeAction.Sell, 45)]
        [TestCase("MSFT", TradeAction.Keep, 0)]
        [TestCase("AAPL", TradeAction.Keep, 0)]
        [TestCase("HD", TradeAction.Keep, 0)]

        public void CalculateAccountAbcReturnsCoorectActionAndShares
            (string stockName, TradeAction expectedAction, int expectedShares) 
        {
            var account = CreateAccountAbc();
            var result = _calculator.Calculate(account);
            var trade = result.For(stockName);
            Assert.That(trade.Action, Is.EqualTo(expectedAction));
            Assert.That(trade.Shares, Is.EqualTo(expectedShares));
        }

        [Test]
        public void CalculateAccountAbcReturnOneTradePerStock() 
        {
            var result = _calculator.Calculate(CreateAccountAbc());
            Assert.That(result.Trades, Has.Count.EqualTo(5));
        }


        [Test]
        public void CalculateAccountAbcSelltoBuy() 
        {
            var result = _calculator.Calculate(CreateAccountAbc());
            Assert.That(result.TotalSellValue, Is.EqualTo(9900m));
            Assert.That(result.TotalSellValue, Is.EqualTo(9900m));
        }

        [Test]
        public void Calculate_Balanced_NoTrades()
        {
            var account = new Account("BAL", 80_000m, new List<Holding>
                {
                    new Holding("AAA", 25, 25, 10),
                    new Holding("BBB", 25, 25, 20),
                    new Holding("CCC", 25, 25, 30),
                    new Holding("DDD", 25, 25, 40)
                });

            var result = _calculator.Calculate(account);

            Assert.That(result.TotalBuyValue, Is.EqualTo(0));
            Assert.That(result.TotalSellValue, Is.EqualTo(0));
            Assert.That(result.Trades.All(t => t.Action == TradeAction.Keep), Is.True);
        }

        [Test]
        public void CalculateMultipleTradesCorrectShares()
        {
           
            var account = new Account("MIX", 50_000m, new List<Holding>
            {
                new Holding("A", 30, 20, 100),
                new Holding("B", 30, 40, 50),
                new Holding("C", 20, 25, 25),
                new Holding("D", 20, 15, 125)
            });

            var result = _calculator.Calculate(account);

            Assert.That(result.For("A").Action, Is.EqualTo(TradeAction.Buy));
            Assert.That(result.For("A").Shares, Is.EqualTo(50));

            Assert.That(result.For("B").Action, Is.EqualTo(TradeAction.Sell));
            Assert.That(result.For("B").Shares, Is.EqualTo(100));

            Assert.That(result.For("C").Action, Is.EqualTo(TradeAction.Sell));
            Assert.That(result.For("C").Shares, Is.EqualTo(100));

            Assert.That(result.For("D").Action, Is.EqualTo(TradeAction.Buy));
            Assert.That(result.For("D").Shares, Is.EqualTo(20));
        }

        [Test]
        public void Calculate_TargetZero_SellsAll()
        {
           
            var account = new Account("EXIT", 10_000m, new List<Holding>
            {
                new Holding("X", 0, 50, 100),
                new Holding("Y", 100, 50, 250)
            });

            var result = _calculator.Calculate(account);

            Assert.That(result.For("X").Action, Is.EqualTo(TradeAction.Sell));
            Assert.That(result.For("X").Shares, Is.EqualTo(50));
            Assert.That(result.For("Y").Action, Is.EqualTo(TradeAction.Buy));
            Assert.That(result.For("Y").Shares, Is.EqualTo(20));
        }

        [Test]
        public void Calculate_Fractional_RoundsDown()
        {
            
            var account = new Account("ROUND", 10_000m, new List<Holding>
            {
                new Holding("P", 60, 50, 300),
                new Holding("Q", 40, 50, 7)
            });

            var result = _calculator.Calculate(account);

            Assert.That(result.For("P").Action, Is.EqualTo(TradeAction.Buy));
            Assert.That(result.For("P").Shares, Is.EqualTo(3));
            Assert.That(result.For("Q").Action, Is.EqualTo(TradeAction.Sell));
            Assert.That(result.For("Q").Shares, Is.EqualTo(142));
        }

        [Test]
        public void CalculateBelowOneShareNoTrade()
        {
      
            var account = new Account("SMALL", 1_000m, new List<Holding>
            {

                new Holding("A", 51, 50, 20),
                new Holding("B", 49, 50, 5)

            });

            var result = _calculator.Calculate(account);

            Assert.That(result.For("A").Action, Is.EqualTo(TradeAction.Keep));
            Assert.That(result.For("A").Shares, Is.EqualTo(0));
            Assert.That(result.For("B").Action, Is.EqualTo(TradeAction.Sell));
            Assert.That(result.For("B").Shares, Is.EqualTo(2));
        }

    }
}
