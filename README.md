# Rebalancer – QA Technical Assessment

My solution for the Charles River Development QA assessment. Written in C# (.NET 10), tests with NUnit.

## Result for account ABC

Only IBM and ORCL are not at target. IBM is 10% under, ORCL is 10% over, so on a $100,000 account that's $10,000 each.

- IBM: $10,000 / $150 = 66.67 → buy **66**
- ORCL: $10,000 / $220 = 45.45 → sell **45**
- MSFT, AAPL, HD: no change

So: sell 45 ORCL ($9,900) and buy 66 IBM ($9,900).

I round down on purpose. You can't trade 0.67 of a share, and buying 67 IBM would cost $10,050, which is more than the ORCL sale gives. After the trades IBM is at 19.9% and ORCL at 20.1%. That's as close to zero variance as whole shares allow.

## Assumptions

- "100% vested" means all money is invested, there is no cash. Buys are paid only from sells.
- Whole shares only, always rounded down.
- Target variance = current % − target % (as in the task). Negative means buy, positive means sell.
- Prices don't change during the rebalance. No fees or taxes.
- If the amount to trade is less than the price of one share, nothing is traded.
- Invalid input (percentages not adding up to 100, price 0, same stock twice, etc.) is rejected and all errors are shown at once.

## How to run

Needs the .NET 10 SDK.

```
dotnet test                            # runs all tests
dotnet run --project Rebalancer.Cli    # prints the result for account ABC
```

## Project structure

- `Rebalancer.Core` – the logic: models, calculator, validation, custom exceptions
- `Rebalancer.Cli` – small console app for a quick manual check
- `Rebalancer.Tests` – NUnit tests
- `docs/Rebalancer_Test_Cases.xlsx` – test cases, with a link to the automated test for each one

## Tests

The task asks to check the number of shares for each security, so most tests do exactly that:

- account ABC from the task (each stock and the totals)
- an account that is already balanced
- several buys and sells at the same time
- target 0% (sell everything)
- rounding down (e.g. 142.86 → 142)
- amount smaller than one share

The expected values are calculated by hand, not with the same formula as the code. Otherwise a bug in the formula would end up in the test too.

There are also tests for the validation: wrong input should fail with a clear message.

GitHub Actions runs all tests on every push.

## Known limitations / next steps

- Because of rounding down, in some accounts the buys can cost a bit more than the sells bring in (example: TC-17 in the test cases). For account ABC this doesn't happen. The next step would be to reduce the buy in that case.
- The stock name check only checks the format, not whether the stock exists (`MSTF` would pass).
- No fractional shares.
- The input is in the code (`Program.cs`). Reading it from a JSON or CSV file would make manual testing easier.

## Use of AI

I built this step by step with an AI assistant, which suggested much of the code. 
I typed and integrated it myself, made the design decisions (structure, naming, which scenarios to test),
ran and debugged everything, and fixed several bugs I found through testing,
for example validation that was never called and a wrong error message.