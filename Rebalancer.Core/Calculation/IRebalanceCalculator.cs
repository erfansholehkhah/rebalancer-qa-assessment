
using Rebalancer.Core.Models;

namespace Rebalancer.Core.Calculation
{
    public interface IRebalanceCalculator
    {
        RebalanceResult Calculate(Account account);
    }
}
