namespace Rebalancer.Core.Exceptions
{
    public class InvalidAccountException : RebalanceException
    {
        public IReadOnlyList<string> Errors { get; }

        public InvalidAccountException(IReadOnlyList<string> errors) 
            : base ("The account is invalid:" + string.Join ("|",errors))
        {
            Errors = errors;   
        }
    }
}
