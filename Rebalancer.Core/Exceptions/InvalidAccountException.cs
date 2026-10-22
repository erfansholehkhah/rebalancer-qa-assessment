using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Rebalancer.Core.Exceptions
{
    internal class InvalidAccountException : RebalanceException
    {
        public IReadOnlyList<string> Errors { get; }

        public InvalidAccountException(IReadOnlyList<string> errors) 
            : base (" the account is invalid: " + string.Join ("|",errors))
        {
            Errors = errors;   
        }
    }
}
