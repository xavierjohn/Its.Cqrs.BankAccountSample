using Microsoft.Its.Domain;

namespace BankAccountDomain
{
    public partial class BankAccount
    {
        public void EnactCommand(Deposit command)
        {
            var created = new Deposited {Amount = command.Amount};
            RecordEvent(created);
        }

        public class Deposit : Command<BankAccount>
        {
            public decimal Amount { get; set; }

            public override bool Authorize(BankAccount target) => true;
        }
    }
}