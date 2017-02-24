using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount
    {
        public class Created : Event<BankAccount>
        {
            public CustomerId CustomerId { get; set; }

            public override void Update(BankAccount aggregate)
            {
            }
        }
    }
}