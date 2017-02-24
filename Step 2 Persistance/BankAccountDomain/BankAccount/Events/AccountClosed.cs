using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount
    {
        public class AccountClosed : Event<BankAccount>
        {
            public override void Update(BankAccount aggregate)
            {
                aggregate.AccountStatus = AccountStatuses.Closed;
            }
        }
    }
}