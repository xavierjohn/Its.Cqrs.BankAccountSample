using System;
using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public class CustomerId : String<CustomerId>
    {
        public CustomerId(string value) : base(value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
        }
    }
}