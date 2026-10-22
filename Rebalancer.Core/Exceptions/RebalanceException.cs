using System;
using System.Collections.Generic;
using System.Text;

namespace Rebalancer.Core.Exceptions
{
    public class RebalanceException : Exception
    {
        public RebalanceException (string messsage) : base (messsage) 
        {

        }

    }
}
