using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount : EventSourcedAggregate<BankAccount>
    {
        public BankAccount(OpenAccount openAccount) : base(openAccount)
        {
        }

        public decimal Balance { get; private set; }
        public AccountStatuses AccountStatus { get; private set; }

        public enum AccountStatuses
        {
            Open,
            Closed
        }
    }
}