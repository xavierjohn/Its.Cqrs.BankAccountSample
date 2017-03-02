using System;
using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount
    {
        public void EnactCommand(OpenAccount command)
        {
            var created = new AccountOpened {CustomerId = command.CustomerId};
            RecordEvent(created);
        }

        public class OpenAccount : ConstructorCommand<BankAccount>
        {
            public OpenAccount(Guid aggregateId) : base(aggregateId)
            {
            }

            public CustomerId CustomerId { get; set; }
        }
    }
}