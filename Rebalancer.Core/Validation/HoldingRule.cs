

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

        //What’s a delegate? A variable that holds a function instead of a value.
        //Func<Holding, bool> = “a function that takes a Holding and returns a bool” (true = valid).


    }
}
