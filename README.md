# Portfolio Rebalancing – QA Technical Assessment

Senior QA Automation Engineer assessment for Charles River Development (State Street).
Built with **C# / .NET 10 / NUnit**.

## 1. The answer

Account ABC has **$100,000**, fully invested (no cash). Each of the 5 stocks should be **20%** of the account.

For each stock:

```
target variance = current % - target %          (negative = buy, positive = sell)
trade amount    = |target variance| / 100 x total assets
shares          = trade amount / unit price, rounded down to whole shares
```

| Stock | Target % | Current % | Variance | Price | Trade amount | Exact shares | **Result** |
|---|---|---|---|---|---|---|---|
| IBM  | 20 | 10 | -10 | $150 | $10,000 | 66.67 | **Buy 66** ($9,900) |
| MSFT | 20 | 20 | 0 | $90 | – | 0 | **Keep** |
| ORCL | 20 | 30 | +10 | $220 | $10,000 | 45.45 | **Sell 45** ($9,900) |
| AAPL | 20 | 20 | 0 | $450 | – | 0 | **Keep** |
| HD   | 20 | 20 | 0 | $70 | – | 0 | **Keep** |

**Sell 45 ORCL and use the $9,900 to buy 66 IBM.** Sells and buys are both $9,900, so no cash is needed and none is left over.

After trading, IBM is at 19.9% and ORCL at 20.1%. Exactly 0% variance is not possible with whole shares, because $10,000 is not a whole number of IBM or ORCL shares. The remaining gap is smaller than the value of one share.

Buying 67 IBM instead would cost $10,050, which is more than the ORCL sale brings in.

## 2. Assumptions

1. **"100% vested" means fully invested:** the account holds no cash. Buys are paid for only by sells.
2. **Whole shares only.** Share counts are **rounded down**, so a trade never goes past the target.
3. **Target variance = current % − target %**, as defined in the assessment.
4. If the trade amount is smaller than the price of one share, the stock is kept (no trade).
5. Prices are fixed during the rebalance. No fees, taxes or minimum lot sizes.
6. Input must be valid, otherwise the whole account is rejected with **all** problems listed:
   - total assets greater than 0, and at least one stock
   - stock name of 1–5 capital letters, optionally a dot and one letter (`IBM`, `BRK.B`)
   - unit price greater than 0
   - target % and current % each between 0 and 100
   - target % and current % each add up to 100 (±0.01 allowed for rounding)
   - each stock listed only once
7. Money and percentages use `decimal`, not `double`, to avoid floating-point rounding errors.

## 3. Project structure

```
Rebalancer.slnx
├── Rebalancer.Core/                  the application logic
│   ├── Models/                       Account, Holding, TradeAction, TradeInstruction, RebalanceResult
│   ├── Calculation/                  IRebalanceCalculator, RebalanceCalculator
│   ├── Validation/                   IAccountValidator, AccountValidator, HoldingRule
│   └── Exceptions/                   RebalanceException, InvalidAccountException
├── Rebalancer.Cli/                   console app: runs account ABC and prints the result
├── Rebalancer.Tests/                 NUnit tests
└── docs/Rebalancer_Test_Cases.xlsx   test cases
```

**Why three projects:** the logic lives in a class library (`Core`), so the tests can call it directly. The console app is only a thin runner for the demo and manual testing.

### OOP concepts used

| Concept | Where |
|---|---|
| Interfaces | `IRebalanceCalculator`, `IAccountValidator` – the calculator and the console app depend on contracts, not concrete classes |
| Inheritance | `InvalidAccountException : RebalanceException : Exception` |
| Delegates | `HoldingRule` stores each check as `Func<Holding, bool>` and its message as `Func<Holding, string>` |
| Lambda expressions | the validation rules and the LINQ queries (`Where`, `Sum`, `Select`, `GroupBy`) |
| Regular expression | stock name format: `^[A-Z]{1,5}(\.[A-Z])?$` |
| Exception handling | custom exceptions, a guard clause, and `try / catch` in the console app |
| Encapsulation | read-only properties, `IReadOnlyList`, private helper methods |
| Dependency injection | the validator is passed into the calculator through its constructor |

## 4. How to run

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
# run all automated tests
dotnet test

# run the console app (account ABC)
dotnet run --project Rebalancer.Cli
```

Output:

```
IBM   Buy    66 shares  $9900
MSFT  Keep    0 shares  $0
ORCL  Sell   45 shares  $9900
AAPL  Keep    0 shares  $0
HD    Keep    0 shares  $0

Total sells: $9900
Total buys:  $9900
```

## 5. Test approach

The main goal, as required by the assessment, is to check that **the number of shares to buy or sell for each stock is correct**.

- **Account ABC:** every stock's action and share count, the sell and buy totals, and one result per stock.
- **More scenarios:** an already balanced account, several buys and sells at once, a target of 0% (sell everything), rounding down (3.33 → 3, 142.86 → 142), and a trade smaller than one share (→ Keep).
- **Validation:** covered by manual test cases (invalid input is rejected with a clear message).

How the tests are written:

- **Expected values are calculated by hand**, not with the production formula. A test that used the same formula as the code would repeat the same bug and still pass. The calculations are shown in the test cases file.
- **Data-driven test** with `[TestCase]` for account ABC: one method, one case per stock.
- **Arrange / Act / Assert** structure, and names in the form `Method_Scenario_ExpectedResult`.

Test cases: [docs/Rebalancer_Test_Cases.xlsx](docs/Rebalancer_Test_Cases.xlsx) (download to open in Excel).

## 6. Known limitations

- **Rounding can make buys cost more than sells bring in.** For account ABC the totals match ($9,900 each), but not for every input. Example: $10,000 account with stock S (target 0%, current 10%, price $300), stock B (target 10%, current 0%, price $10) and stock C (target 90%, current 90%, price $50). S sells 3 shares = $900, while B buys 100 shares = $1,000. With no cash in the account, the buy should be reduced to 90 shares. This rule is not implemented yet.
- **The stock name check only checks the format.** A name like `MSTF` (a typo for `MSFT`) passes, because it is a valid format. Catching it would need a list of known stocks.
- **Fractional shares are not supported.**

## 7. What I would add next

- Enforce the cash rule above, with tests for it.
- An option for fractional shares.
- Read accounts from a file (JSON or CSV) instead of the code, so testers can add scenarios without changing code.
- A REST API around the calculator, with API tests.
- Store accounts and trades in a database, and validate the results with SQL.
- Performance tests with large accounts (thousands of stocks).
