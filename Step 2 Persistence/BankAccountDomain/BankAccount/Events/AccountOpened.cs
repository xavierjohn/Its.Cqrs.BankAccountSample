using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount
    {
        public class AccountOpened : Event<BankAccount>
        {
            public CustomerId CustomerId { get; set; }

            public override void Update(BankAccount aggregate)
            {
                aggregate.AccountStatus = AccountStatuses.Open;
            }
        }
    }
}