using Rebalancer.Core.Exceptions;
using Rebalancer.Core.Models;
using Rebalancer.Core.Validation;

namespace Rebalancer.Tests;

[TestFixture]
public class AccountValidatorTests
{
    private readonly IAccountValidator _validator = new AccountValidator();

    [Test]
    public void Validate_ValidAccount_NoError_Test()
    {
        var account = new Account("OK", 10_000m, new List<Holding>
        {
            new Holding("AAA", 50, 40, 10),
            new Holding("BBB", 50, 60, 20)
        });

        Assert.DoesNotThrow(() => _validator.Validate(account));
    }

    [Test]
    public void Validate_PriceZero_Throws_Test()
    {
        var account = new Account("BAD", 10_000m, new List<Holding>
        {
            new Holding("AAA", 50, 40, 0),
            new Holding("BBB", 50, 60, 20)
        });

        var ex = Assert.Throws<InvalidAccountException>(() => _validator.Validate(account));

        Assert.That(ex.Message, Does.Contain("AAA: unit price must be greater than 0 (was 0)"));
    }

    [TestCase("ibm")]
    [TestCase("IBM1")]
    [TestCase("TOOLONG")]
    public void Validate_InvalidStockName_Throws(string stockName)
    {
        var account = new Account("BAD", 10_000m, new List<Holding>
        {
            new Holding(stockName, 50, 50, 10),
            new Holding("BBB", 50, 50, 10)
        });

        var ex = Assert.Throws<InvalidAccountException>(() => _validator.Validate(account));

        Assert.That(ex.Message, Does.Contain("stock name must be 1-5 capital letters"));
    }

    [Test]
    public void Validate_TargetsNot100_Throws_Test()
    {
       
        var account = new Account("BAD", 10_000m, new List<Holding>
        {
            new Holding("AAA", 60, 50, 10),
            new Holding("BBB", 50, 50, 10)
        });

        var ex = Assert.Throws<InvalidAccountException>(() => _validator.Validate(account));

        Assert.That(ex.Message, Does.Contain("Target % must add up to 100 (was 110)"));
    }

    [Test]
    public void Validate_CurrentNot100_Throws_Test()
    {
        
        var account = new Account("BAD", 10_000m, new List<Holding>
        {
            new Holding("AAA", 50, 30, 10),
            new Holding("BBB", 50, 30, 10)
        });

        var ex = Assert.Throws<InvalidAccountException>(() => _validator.Validate(account));

        Assert.That(ex.Message, Does.Contain("Current % must add up to 100 (was 60)"));
    }

    [Test]
    public void Validate_DuplicateStock_Throws_Test()
    {
        var account = new Account("BAD", 10_000m, new List<Holding>
        {
            new Holding("IBM", 50, 50, 150),
            new Holding("IBM", 50, 50, 150)
        });

        var ex = Assert.Throws<InvalidAccountException>(() => _validator.Validate(account));

        Assert.That(ex.Message, Does.Contain("IBM: stock appears more than once"));
    }

    [Test]
    public void Validate_TotalAssetsZero_Throws_Test()
    {
        var account = new Account("BAD", 0, new List<Holding>
        {
            new Holding("AAA", 50, 50, 10),
            new Holding("BBB", 50, 50, 10)
        });

        var ex = Assert.Throws<InvalidAccountException>(() => _validator.Validate(account));

        Assert.That(ex.Message, Does.Contain("Total assets must be greater than 0"));
    }

    [Test]
    public void Validate_SeveralProblems_ReportsAll_Test()
    {
       
        var account = new Account("BAD", 0, new List<Holding>
        {
            new Holding("AAA", 70, 50, 0),
            new Holding("BBB", 50, 50, 10)
        });

        var ex = Assert.Throws<InvalidAccountException>(() => _validator.Validate(account));

        Assert.That(ex.Errors, Has.Count.EqualTo(3));
    }
}