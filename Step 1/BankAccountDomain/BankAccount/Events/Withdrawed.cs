using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount
    {
        public class Withdrawed : Event<BankAccount>
        {
            public decimal Amount { get; set; }

            public override void Update(BankAccount aggregate)
            {
                aggregate.Balance -= Amount;
            }
        }
    }
}