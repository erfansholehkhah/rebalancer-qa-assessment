

using Rebalancer.Core.Models;

namespace Rebalancer.Core.Validation
{
    public class HoldingRule
    {

        public Func<Holding, bool> IsValid { get; }
        public Func<Holding, string> ErrorMessage { get; }

        public HoldingRule (Func <Holding, bool> isValid , Func<Holding,string> errorMessage ) 
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

    }
}
