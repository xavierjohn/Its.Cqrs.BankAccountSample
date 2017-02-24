using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount : EventSourcedAggregate<BankAccount>
    {
        public decimal Balance { get; private set; }
        public BankAccount(Create create): base(create)
        {
        }
    }
}
